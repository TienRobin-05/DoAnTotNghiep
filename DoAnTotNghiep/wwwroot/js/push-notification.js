(() => {
    var toggle = document.getElementById("tuDongNhacLichSwitch");

    if (!window.isSecureContext || !("serviceWorker" in navigator) || !("PushManager" in window) || !("Notification" in window)) {
        if (toggle) { toggle.checked = false; toggle.disabled = true; }
        return;
    }

    // ===== SERVICE CHUNG: KIEM TRA VA DAY NOTIFICATION RA DESKTOP =====
    var desktopPushStarted = false;
    var NGUONG_GOP = 3;

    async function checkAndPushUnreadNotifications() {
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

            var shownIds = getShownNotificationIds();

            // Loc nhung thong bao chua tung day desktop
            var newItems = [];
            for (var i = 0; i < items.length; i++) {
                if (shownIds.indexOf(items[i].id) < 0) {
                    newItems.push(items[i]);
                }
            }

            if (newItems.length === 0) {
                console.log("[DesktopPush] Tat ca thong bao da duoc day truoc do.");
                return;
            }

            console.log("[DesktopPush] Co " + newItems.length + " thong bao moi can day ra desktop.");

            // Phan loai: qua han va cac loai khac
            var overdueItems = [];
            var otherItems = [];
            for (var i = 0; i < newItems.length; i++) {
                if (newItems[i].category === "overdue") {
                    overdueItems.push(newItems[i]);
                } else {
                    otherItems.push(newItems[i]);
                }
            }

            var allPushedIds = [];

            // Xu ly thong bao qua han: gom theo ho so
            var overdueByProfile = {};
            for (var i = 0; i < overdueItems.length; i++) {
                var item = overdueItems[i];
                var key = item.maHoSo ? String(item.maHoSo) : "unknown";
                if (!overdueByProfile[key]) {
                    overdueByProfile[key] = {
                        hoTenHoSo: item.hoTenHoSo || "Hồ sơ",
                        items: []
                    };
                }
                overdueByProfile[key].items.push(item);
            }

            for (var profileKey in overdueByProfile) {
                var profile = overdueByProfile[profileKey];
                if (profile.items.length > NGUONG_GOP) {
                    // Day 1 thong bao tong hop
                    var body = "Hồ sơ " + profile.hoTenHoSo + " có " + profile.items.length + " mũi tiêm quá hạn. Vui lòng vào lịch tiêm để kiểm tra và cập nhật trạng thái.";
                    await showDesktopNotification(registration, {
                        title: "Pharmacy City",
                        message: body,
                        url: "/LichTiem/ChonHoSo",
                        id: profile.items[0].id
                    });
                    for (var j = 0; j < profile.items.length; j++) {
                        allPushedIds.push(profile.items[j].id);
                    }
                    await wait(700);
                } else {
                    // Day tung thong bao rieng
                    for (var j = 0; j < profile.items.length; j++) {
                        var item = profile.items[j];
                        await showDesktopNotification(registration, item);
                        allPushedIds.push(item.id);
                        await wait(700);
                    }
                }
            }

            // Xu ly cac thong bao khac (den lich, sap den, da cap nhat)
            if (otherItems.length > NGUONG_GOP) {
                var body = "Bạn có " + otherItems.length + " thông báo tiêm chủng cần kiểm tra. Vui lòng vào mục Thông báo để xem chi tiết.";
                await showDesktopNotification(registration, {
                    title: "Pharmacy City",
                    message: body,
                    url: "/ThongBao/Index",
                    id: otherItems[0].id
                });
                for (var j = 0; j < otherItems.length; j++) {
                    allPushedIds.push(otherItems[j].id);
                }
                await wait(700);
            } else {
                for (var j = 0; j < otherItems.length; j++) {
                    var item = otherItems[j];
                    await showDesktopNotification(registration, item);
                    allPushedIds.push(item.id);
                    await wait(700);
                }
            }

            // Luu vao localStorage de chong trung khi reload
            for (var i = 0; i < allPushedIds.length; i++) {
                addShownNotificationId(allPushedIds[i]);
            }

            // Danh dau da day tren server
            if (allPushedIds.length > 0) {
                await fetch("/api/notifications/mark-desktop-pushed", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify({ notificationIds: allPushedIds })
                });
                console.log("[DesktopPush] Da day " + allPushedIds.length + " thong bao ra desktop.");
            }
        } catch (error) {
            console.error("[DesktopPush] Loi:", error);
        }
    }

    function getShownNotificationIds() {
        try {
            var stored = localStorage.getItem("desktopNotificationShownIds");
            return stored ? JSON.parse(stored) : [];
        } catch (e) {
            return [];
        }
    }

    function addShownNotificationId(id) {
        try {
            var ids = getShownNotificationIds();
            if (ids.indexOf(id) < 0) {
                ids.push(id);
                localStorage.setItem("desktopNotificationShownIds", JSON.stringify(ids));
            }
        } catch (e) {
            // silent
        }
    }

    async function showDesktopNotification(registration, item) {
        var title = item.title || "Pharmacy City";
        var body = item.message || "";
        var tag = "notif-" + (item.id || Date.now());

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

    // ===== TU DONG CHAY KHI APP START =====
    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", checkAndPushUnreadNotifications);
    } else {
        checkAndPushUnreadNotifications();
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

        // Day ngay thong bao desktop sau khi bat cong tac (khong can vao trang Thong bao)
        desktopPushStarted = false;
        await checkAndPushUnreadNotifications();
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
