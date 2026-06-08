(() => {
    const checkbox = document.getElementById("btnBatThongBao");
    if (!checkbox) return;

    if (!window.isSecureContext || !("serviceWorker" in navigator) || !("PushManager" in window) || !("Notification" in window)) {
        checkbox.checked = false;
        checkbox.disabled = true;
        return;
    }

    capNhatTrangThaiBanDau();

    checkbox.addEventListener("change", async () => {
        const muonBatThongBao = checkbox.checked;
        checkbox.disabled = true;

        try {
            if (muonBatThongBao) {
                await batThongBao();
                checkbox.checked = true;
                alert("Da bat tu dong nhac lich.");
            } else {
                await tatThongBao();
                checkbox.checked = false;
                alert("Da tat tu dong nhac lich.");
            }
        } catch (error) {
            checkbox.checked = !muonBatThongBao;
            alert(`Khong cap nhat duoc thong bao: ${layThongBaoLoi(error)}`);
        } finally {
            checkbox.disabled = Notification.permission === "denied";
        }
    });

    async function capNhatTrangThaiBanDau() {
        checkbox.checked = false;
        checkbox.disabled = true;

        if (Notification.permission === "denied") {
            checkbox.disabled = true;
            return;
        }

        if (Notification.permission === "granted") {
            const registration = await layServiceWorkerDangHoatDong();
            const subscription = await registration.pushManager.getSubscription();
            checkbox.checked = !!subscription;

            if (subscription) {
                await luuSubscription(subscription);
            }
        }

        checkbox.disabled = false;
    }

    async function batThongBao() {
        const permission = await Notification.requestPermission();
        if (permission !== "granted") {
            throw new Error("Ban can cho phep thong bao trong trinh duyet.");
        }

        const publicKeyResponse = await fetch("/Push/PublicKey");
        const publicKeyData = await publicKeyResponse.json();
        const registration = await layServiceWorkerDangHoatDong();
        const applicationServerKey = chuyenBase64UrlThanhUint8Array(publicKeyData.publicKey);

        let subscription = await registration.pushManager.getSubscription();
        if (!subscription) {
            subscription = await registration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey
            });
        }

        await luuSubscription(subscription);
    }

    async function layServiceWorkerDangHoatDong() {
        const registration = await navigator.serviceWorker.register("/push-service-worker.js", {
            scope: "/",
            updateViaCache: "none"
        });

        await registration.update();

        if (registration.active) {
            return registration;
        }

        const worker = registration.installing || registration.waiting;
        if (worker) {
            await doiServiceWorkerActive(worker);
        }

        return await navigator.serviceWorker.ready;
    }

    function doiServiceWorkerActive(worker) {
        return new Promise((resolve, reject) => {
            if (worker.state === "activated") {
                resolve();
                return;
            }

            const timeout = setTimeout(() => reject(new Error("Service Worker chua san sang. Hay tai lai trang va thu lai.")), 10000);

            worker.addEventListener("statechange", () => {
                if (worker.state === "activated") {
                    clearTimeout(timeout);
                    resolve();
                }
            });
        });
    }

    async function tatThongBao() {
        const registration = await navigator.serviceWorker.ready;
        const subscription = await registration.pushManager.getSubscription();

        if (!subscription) {
            return;
        }

        await fetch("/Push/HuyDangKy", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(subscription)
        });

        await subscription.unsubscribe();
    }

    async function luuSubscription(subscription) {
        const response = await fetch("/Push/DangKy", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(subscription)
        });

        if (!response.ok) {
            throw new Error(`Khong luu duoc dang ky thong bao. Ma loi: ${response.status}`);
        }
    }

    function layThongBaoLoi(error) {
        const noiDungLoi = error?.message || error?.name || "loi khong xac dinh";

        if (noiDungLoi.toLowerCase().includes("push service")) {
            return "Trinh duyet dang chan dich vu Push. Hay thu bang Chrome/Edge, hoac bat dich vu Push Messaging trong Brave.";
        }

        return noiDungLoi;
    }

    function chuyenBase64UrlThanhUint8Array(base64Url) {
        const padding = "=".repeat((4 - base64Url.length % 4) % 4);
        const base64 = (base64Url + padding).replace(/-/g, "+").replace(/_/g, "/");
        const rawData = window.atob(base64);
        const output = new Uint8Array(rawData.length);

        for (let i = 0; i < rawData.length; i++) {
            output[i] = rawData.charCodeAt(i);
        }

        return output;
    }
})();
