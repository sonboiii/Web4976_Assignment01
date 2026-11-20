(() => {
    const root = document.documentElement;
    try { root.dataset.bsTheme = localStorage.getItem("civic-theme") || "light"; } catch {}
    const key = (tenant, list) => `civic:${tenant}:${list}`;
    window.civic = {
        watchSession(username) {
            const check = async () => {
                try {
                    const response = await fetch("/api/viewer", { cache: "no-store" });
                    if (response.ok && (await response.json()).username !== username) location.reload();
                } catch {}
            };
            window.addEventListener("focus", check);
            window.addEventListener("pageshow", check);
            if ("BroadcastChannel" in window) {
                const channel = new BroadcastChannel("civic-session");
                channel.onmessage = check;
            }
        },
        clearGuides() {
            for (let i = sessionStorage.length - 1; i >= 0; i--) {
                const name = sessionStorage.key(i);
                if (/^civic:\d+:guide$/.test(name)) sessionStorage.removeItem(name);
            }
        },
        theme() {
            const next = root.dataset.bsTheme === "dark" ? "light" : "dark";
            root.dataset.bsTheme = next;
            try { localStorage.setItem("civic-theme", next); } catch {}
        },
        read(tenant, list) {
            const raw = sessionStorage.getItem(key(tenant, list));
            if (!raw) return [];
            try {
                const ids = JSON.parse(raw);
                return Array.isArray(ids) ? ids.filter(id => Number.isInteger(id) && id > 0 && id <= 2147483647) : [];
            } catch { return []; }
        },
        write(tenant, list, ids) {
            if (ids.length) sessionStorage.setItem(key(tenant, list), JSON.stringify(ids));
            else sessionStorage.removeItem(key(tenant, list));
        }
    };
})();
