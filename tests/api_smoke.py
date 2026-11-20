"""Integration checks for a running, locally seeded MEMS API (Python standard library)."""
import json
import os
from contextlib import ExitStack
from urllib.error import HTTPError
from urllib.request import Request, urlopen
from uuid import uuid4

ORIGIN = os.getenv("MEMS_API", "http://localhost:5181").rstrip("/")
PASSWORD = os.getenv("MEMS_DEMO_PASSWORD", "P@$$w0rd")


def request(method, path, *, token=None, body=None, expected=200, media="application/json"):
    headers = {}
    if token:
        headers["Authorization"] = "Bearer " + token
    data = None
    if body is not None:
        headers["Content-Type"] = media
        data = body if isinstance(body, bytes) else json.dumps(body).encode()
    req = Request(ORIGIN + path, data=data, headers=headers, method=method)
    try:
        response = urlopen(req, timeout=15)
    except HTTPError as error:
        response = error
    with response:
        raw = response.read()
        assert response.status == expected, f"{method} {path}: expected {expected}, got {response.status}"
        if raw and "json" in response.headers.get("Content-Type", ""):
            return json.loads(raw)
        return raw


def login(username):
    return request("POST", "/api/account/login", body={"username": username, "password": PASSWORD})["accessToken"]


def main():
    root, manager, pm = login("super"), login("burnabyadmin"), login("pm")
    towns = request("GET", "/api/municipalities")
    assert {"burnaby", "surrey", "richmond", "pitt-meadows"} <= {m["slug"] for m in towns}
    assert [m["slug"] for m in request("GET", "/api/municipalities", token=manager)] == ["burnaby"]
    burnaby = request("GET", "/api/Burnaby/municipality")
    surrey_candidate = request("GET", "/api/surrey/candidates")[0]
    request("GET", f"/api/burnaby/candidates/{surrey_candidate['id']}", expected=404)
    request("GET", "/api/surrey/candidates", token=manager, expected=404)
    request("GET", "/api/administrators", token=manager, expected=403)
    request("GET", "/api/dashboard", expected=401)
    assert request("GET", "/api/dashboard", token=manager)["municipalities"] == 1
    assert all(c["municipalityId"] == burnaby["id"] for c in request("GET", "/api/burnaby/candidates"))
    pitt = request("GET", "/api/municipalities", token=pm)
    assert [m["slug"] for m in pitt] == ["pitt-meadows"]
    pitt_candidates = request("GET", "/api/pitt-meadows/candidates", token=pm)
    assert pitt_candidates and all(c["municipalityId"] == pitt[0]["id"] for c in pitt_candidates)
    assert request("GET", "/api/dashboard", token=pm)["candidates"] == len(pitt_candidates)
    for path in ["/api/burnaby/municipality", "/api/burnaby/positions", "/api/burnaby/candidates",
                 f"/api/municipalities/{burnaby['id']}", f"/api/surrey/candidates/{surrey_candidate['id']}/photo"]:
        request("GET", path, token=pm, expected=404)
    print("PASS Pitt Meadows manager sees only assigned municipality, positions and candidates")
    print("PASS seeded accounts, dashboard and read isolation")

    slug = "test-" + uuid4().hex
    form = {"name": slug, "slug": slug, "description": "Smoke test", "province": "BC",
            "electionDate": "2026-10-17", "accent": "#236559"}
    base = "/api/" + slug
    with ExitStack() as cleanup:
        def create(path, body):
            record = request("POST", path, token=root, body=body, expected=201)
            cleanup.callback(request, "DELETE", path + "/" + str(record["id"]), token=root, expected=204)
            return record
        request("POST", "/api/municipalities", token=manager, body=form, expected=403)
        town = create("/api/municipalities", form)
        request("PUT", "/api/municipalities/" + str(town["id"]), token=root, body={**form, "description": "Updated"})
        assert request("GET", base + "/municipality")["description"] == "Updated"
        request("POST", base + "/positions", token=root, body={"name": "Mayor", "seatsAvailable": 0}, expected=400)
        office = create(base + "/positions", {"name": "Mayor", "seatsAvailable": 1})
        position_path = base + "/positions/" + str(office["id"])
        request("PUT", position_path, token=root, body={"name": "Councillor", "seatsAvailable": 2})
        assert request("GET", position_path)["seatsAvailable"] == 2
        profile = {"name": "Sample", "positionId": office["id"], "profile": "Fictional profile"}
        request("POST", "/api/burnaby/candidates", token=manager, body=profile, expected=400)
        c = create(base + "/candidates", profile)
        candidate_path = base + "/candidates/" + str(c["id"])
        request("PUT", candidate_path, token=root, body={**profile, "name": "Sample Updated"})
        assert request("GET", candidate_path)["name"] == "Sample Updated"
        assert request("GET", base + "/candidates?search=Updated")[0]["id"] == c["id"]
        request("GET", base + "/logo")
        request("GET", candidate_path + "/photo")
        request("PUT", candidate_path + "/photo", token=root, body=b"invalid image", media="image/png", expected=400)
        request("DELETE", position_path, token=root, expected=409)
        request("DELETE", "/api/municipalities/" + str(town["id"]), token=root, expected=409)
        account = create("/api/administrators", {"username": slug, "password": PASSWORD, "municipalityId": town["id"]})
        assert request("GET", "/api/account/me", token=login(account["username"]))["municipalityId"] == town["id"]
    request("GET", base + "/municipality", expected=404)
    print("PASS CRUD, validation, tenant assignment, media and deletion rules")


if __name__ == "__main__":
    main()
