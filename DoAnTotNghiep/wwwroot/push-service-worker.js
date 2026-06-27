self.addEventListener("install", event => {
    event.waitUntil(self.skipWaiting());
});

self.addEventListener("activate", event => {
    event.waitUntil(self.clients.claim());
});

self.addEventListener("push", event => {
    if (!event.data) return;

    const data = event.data.json();

    const title = data.title || "Pharmacity";
    const options = {
        body: data.body || "",
        icon: data.icon || "/images/logo/pharmacity-favicon.png",
        badge: data.badge || "/images/logo/pharmacity-favicon.png",
        data: data.data || {},
        tag: data.tag || "pharmacy-city-notification",
        renotify: true,
        requireInteraction: false
    };

    event.waitUntil(self.registration.showNotification(title, options));
});

self.addEventListener("notificationclick", event => {
    event.notification.close();

    const urlToOpen = event.notification.data?.url || "/ThongBao/Index";

    event.waitUntil(
        clients.matchAll({
            type: "window",
            includeUncontrolled: true
        }).then(function (clientList) {
            for (const client of clientList) {
                if ("focus" in client) {
                    client.navigate(urlToOpen);
                    return client.focus();
                }
            }

            if (clients.openWindow) {
                return clients.openWindow(urlToOpen);
            }
        })
    );
});
