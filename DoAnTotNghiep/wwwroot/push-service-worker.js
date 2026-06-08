self.addEventListener("install", event => {
    event.waitUntil(self.skipWaiting());
});

self.addEventListener("activate", event => {
    event.waitUntil(self.clients.claim());
});

self.addEventListener("push", event => {
    let data = {};

    if (event.data) {
        data = event.data.json();
    }

    const title = data.title || "Thong bao";
    const options = {
        body: data.body || "",
        icon: data.icon || "/favicon.ico",
        data: {
            url: data.url || "/ThongBao/Index"
        }
    };

    event.waitUntil(self.registration.showNotification(title, options));
});

self.addEventListener("notificationclick", event => {
    event.notification.close();

    const url = event.notification.data?.url || "/ThongBao/Index";
    event.waitUntil(self.clients.openWindow(url));
});
