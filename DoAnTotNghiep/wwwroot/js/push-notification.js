(() => {
    var toggle = document.getElementById("tuDongNhacLichSwitch");

    if (!window.isSecureContext || !("serviceWorker" in navigator) || !("PushManager" in window) || !("Notification" in window)) {
        if (toggle) { toggle.checked = false; toggle.disabled = true; }
        return;
    }

    // ===== KHI VAO APP: TU DONG DAY THONG BAO CHUA DOC RA DESKTOP =====
    var desktopPushStarted = false;

    async function initDesktopUnreadNotifications() {
        if (desktopPushStarted) return;
        desktopPushStarted = true;

        try {
            var pushRes = await fetch("/api/reminders/push-enabled", { credentials: "include" });
            if (!pushRes.ok) return;
            var pushSetting = await pushRes.json();
            if (!pushSetting.enabled) {
                console.log("[DesktopPush] User chua bat tu dong nhac lich.");
                return;
            }

            if (Notification.permission !== "granted") {
                console.log("[DesktopPush] Chua cap quyen notification.");
                return;
            }

            var registration = await navigator.serviceWorker.ready;

            var unreadRes = await fetch("/api/notifications/unread-for-push", { credentials: "include" });
            if (!unreadRes.ok) return;
            var unreadData = await unreadRes.json();
            var items = Array.isArray(unreadData.items) ? unreadData.items : [];

            if (items.length === 0) {
                console.log("[DesktopPush] Khong co thong bao chua doc can day.");
                return;
            }

            console.log("[DesktopPush] Dang day " + items.length + " thong bao chua doc ra desktop...");

            var pushedIds = [];

            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                await showDesktopNotification(registration, item);
                pushedIds.push(item.id);
                await wait(700);
            }

            if (pushedIds.length > 0) {
                await fetch("/api/notifications/mark-desktop-pushed", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify({ notificationIds: pushedIds })
                });
                console.log("[DesktopPush] Da day " + pushedIds.length + " thong bao ra desktop.");
            }
        } catch (error) {
            console.error("[DesktopPush] Loi:", error);
        }
    }

    async function showDesktopNotification(registration, item) {
        var title = "Pharmacy City";
        var body = (item.title || "Thong bao") + "\n" + (item.message || "");
        var tag = "notif-" + item.id;

        try {
            await registration.showNotification(title, {
                body: body,
                icon: "/images/logo/pharmacy-favicon.png",
                badge: "/images/logo/pharmacy-favicon.png",
                tag: tag,
                renotify: true,
                requireInteraction: false,
                data: {
                    url: item.url || "/ThongBao/Index",
                    notificationId: item.id,
                    category: item.category
                }
            });
        } catch (e) {
            console.warn("[DesktopPush] showNotification loi:", e);
        }
    }

    function wait(ms) {
        return new Promise(function (resolve) { setTimeout(resolve, ms); });
    }

    // Tu dong chay khi DOM ready
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initDesktopUnreadNotifications);
    } else {
        initDesktopUnreadNotifications();
    }

    // ===== CONG TAC BAT/TAT TREN TRANG CHU =====
    if (!toggle) return;

    capNhatTrangThaiBanDau().catch(function () {
        toggle.checked = false;
        toggle.disabled = Notification.permission === "denied";
    });

    toggle.addEventListener("change", async function () {
        var muonBat = toggle.checked;
        toggle.disabled = true;

        try {
            if (muonBat) {
                await batThongBao();
                toggle.checked = true;
            } else {
                await tatThongBao();
                toggle.checked = false;
            }
        } catch (error) {
            toggle.checked = !muonBat;
            alert("Loi khi cap nhat thong bao: " + layThongBaoLoi(error));
        } finally {
            toggle.disabled = Notification.permission === "denied";
        }
    });

    async function capNhatTrangThaiBanDau() {
        toggle.checked = false;
        toggle.disabled = true;

        try {
            var res = await fetch("/api/reminders/push-enabled", { credentials: "include" });
            if (res.ok) {
                var data = await res.json();
                toggle.checked = data.enabled === true;
            }
        } catch (e) {
            console.log("[Push] Khong lay duoc push setting:", e);
        }

        if (Notification.permission === "denied") {
            toggle.checked = false;
            toggle.disabled = true;
            return;
        }

        toggle.disabled = false;
    }

    async function batThongBao() {
        if (Notification.permission === "denied") {
            throw new Error("Trinh duyet da chan thong bao. Hay vao Settings > Privacy > Notifications de cho phep.");
        }

        var permission = Notification.permission;

        if (permission === "default") {
            permission = await Notification.requestPermission();
        }

        if (permission !== "granted") {
            throw new Error("Ban chua cho phep thong bao. Hay chon Allow trong hop thoai.");
        }

        console.log("[Push] Notification.permission:", permission);

        var publicKeyRes = await fetch("/Push/PublicKey");
        if (!publicKeyRes.ok) {
            throw new Error("Khong lay duoc khoa dang ky thong bao.");
        }

        var publicKeyData = await publicKeyRes.json();
        if (!publicKeyData.publicKey) {
            throw new Error("May chu chua cau hinh khoa Web Push.");
        }

        console.log("[Push] Registering service worker...");

        var registration = await navigator.serviceWorker.register("/push-service-worker.js", {
            scope: "/",
            updateViaCache: "none"
        });

        await navigator.serviceWorker.ready;

        console.log("[Push] Service worker registered");

        var applicationServerKey = urlBase64ToUint8Array(publicKeyData.publicKey);

        var subscription = await registration.pushManager.getSubscription();

        if (!subscription) {
            subscription = await registration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerKey
            });
        }

        console.log("[Push] Subscription:", subscription);

        var subscribeRes = await fetch("/Push/DangKy", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(subscription)
        });

        if (!subscribeRes.ok) {
            throw new Error("Khong luu duoc push subscription. Ma loi: " + subscribeRes.status);
        }

        console.log("[Push] Subscribe API response OK");

        var settingRes = await fetch("/api/reminders/push-enabled", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ enabled: true })
        });

        if (!settingRes.ok) {
            throw new Error("Khong luu duoc trang thai bat nhac lich.");
        }

        console.log("[Push] Da bat thong bao day.");
    }

    async function tatThongBao() {
        var settingRes = await fetch("/api/reminders/push-enabled", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ enabled: false })
        });

        if (!settingRes.ok) {
            throw new Error("Khong luu duoc trang thai tat nhac lich.");
        }

        console.log("[Push] Da tat thong bao day.");
    }

    function layThongBaoLoi(error) {
        var msg = error?.message || error?.name || "Loi khong xac dinh";
        if (msg.toLowerCase().includes("push service")) {
            return "Trinh duyet dang chan dich vu Push. Hay thu bang Chrome/Edge.";
        }
        return msg;
    }

    function urlBase64ToUint8Array(base64String) {
        var padding = "=".repeat((4 - base64String.length % 4) % 4);
        var base64 = (base64String + padding).replace(/-/g, "+").replace(/_/g, "/");
        var rawData = window.atob(base64);
        var outputArray = new Uint8Array(rawData.length);
        for (var i = 0; i < rawData.length; ++i) {
            outputArray[i] = rawData.charCodeAt(i);
        }
        return outputArray;
    }
})();
