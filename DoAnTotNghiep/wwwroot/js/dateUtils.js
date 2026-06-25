// === Hàm tiện ích format ngày tháng Việt Nam ===
// Định dạng: DD/MM/YYYY

function formatDateVN(value) {
    if (!value) return "";
    var raw = String(value);
    var dateOnly = raw.indexOf("T") >= 0 ? raw.split("T")[0] : raw;

    if (/^\d{4}-\d{2}-\d{2}$/.test(dateOnly)) {
        var parts = dateOnly.split("-");
        return parts[2] + "/" + parts[1] + "/" + parts[0];
    }

    if (/^\d{2}\/\d{2}\/\d{4}$/.test(raw)) {
        return raw;
    }

    return raw;
}

function formatDateTimeVN(value) {
    if (!value) return "";
    var raw = String(value);
    var datePart = raw.indexOf("T") >= 0 ? raw.split("T")[0] : raw;
    var timePart = raw.indexOf("T") >= 0 ? raw.split("T")[1] : "";

    var formattedDate = formatDateVN(datePart);

    if (!timePart) return formattedDate;

    var time = timePart.length >= 5 ? timePart.slice(0, 5) : timePart;
    return formattedDate + " " + time;
}

function normalizeDateForInput(value) {
    if (!value) return "";
    var raw = String(value);

    if (raw.indexOf("T") >= 0) {
        return raw.split("T")[0];
    }

    if (/^\d{4}-\d{2}-\d{2}$/.test(raw)) {
        return raw;
    }

    if (/^\d{2}\/\d{2}\/\d{4}$/.test(raw)) {
        var parts = raw.split("/");
        return parts[2] + "-" + parts[1] + "-" + parts[0];
    }

    return "";
}

function convertVNDateToApi(value) {
    if (!value) return "";
    if (/^\d{4}-\d{2}-\d{2}$/.test(value)) {
        return value;
    }

    if (/^\d{2}\/\d{2}\/\d{4}$/.test(value)) {
        var parts = value.split("/");
        return parts[2] + "-" + parts[1] + "-" + parts[0];
    }

    return value;
}
