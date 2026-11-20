const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const vm = require("node:vm");
const script = fs.readFileSync(path.join(__dirname, "../MEMS.Public/wwwroot/assets/civic.js"), "utf8");

function storage() {
    const entries = new Map();
    return {
        get length() { return entries.size; },
        key: index => [...entries.keys()][index],
        getItem: key => entries.get(key) ?? null,
        setItem: (key, value) => entries.set(key, value),
        removeItem: key => entries.delete(key),
        entries
    };
}

function tab(localStorage, sessionStorage) {
    const window = {};
    vm.runInNewContext(script, { window, document: { documentElement: { dataset: {} } }, sessionStorage, localStorage });
    return window.civic;
}

test("guides survive refresh, stay scoped to a tenant and disappear with the session", () => {
    const local = storage(), session = storage();
    const page = tab(local, session);
    page.write(3, "guide", [13]);
    page.write(3, "compare", [14]);
    page.theme();
    assert.deepEqual([...tab(local, session).read(3, "guide")], [13]);
    assert.deepEqual([...page.read(4, "guide")], []);
    assert.deepEqual([...tab(local, storage()).read(3, "guide")], []);
    assert.deepEqual([...local.entries.keys()], ["civic-theme"]);
    page.write(3, "guide", []);
    assert.equal(session.entries.has("civic:3:guide"), false);
    assert.deepEqual([...page.read(3, "compare")], [14]);
});

test("a damaged session entry leaves the directory usable", () => {
    const session = storage(), page = tab(storage(), session);
    session.setItem("civic:5:guide", "bad json");
    assert.deepEqual([...page.read(5, "guide")], []);
    session.setItem("civic:5:guide", '[null,0,"3",4,99999999999]');
    assert.deepEqual([...page.read(5, "guide")], [4]);
});

test("a visitor can keep choices in multiple cities and clear only voting guides", () => {
    const local = storage(), session = storage();
    const visitor = tab(local, session);
    visitor.write(1, "guide", [1]);
    visitor.write(3, "guide", [13]);
    visitor.write(1, "compare", [2]);
    assert.deepEqual([...visitor.read(1, "guide")], [1]);
    assert.deepEqual([...visitor.read(3, "guide")], [13]);
    visitor.clearGuides();
    assert.deepEqual([...session.entries.keys()], ["civic:1:compare"]);
});
