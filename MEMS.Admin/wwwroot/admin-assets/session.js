if ("BroadcastChannel" in window) {
    const channel = new BroadcastChannel("civic-session");
    channel.postMessage("check");
    channel.close();
}
