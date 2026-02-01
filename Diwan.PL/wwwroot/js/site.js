document.addEventListener("DOMContentLoaded", function () {
    const toggleBtn = document.getElementById("theme-toggle");
    const icon = document.getElementById("theme-icon");
    const label = document.getElementById("theme-label");
    const body = document.body;

    if (localStorage.getItem("theme") === "dark") {
        body.classList.add("dark-mode");
        icon.classList.replace("fa-moon", "fa-sun");
        label.textContent = "الوضع الفاتح";
    }

    toggleBtn.addEventListener("click", function () {
        body.classList.toggle("dark-mode");
        const isDark = body.classList.contains("dark-mode");

        if (isDark) {
            icon.classList.replace("fa-moon", "fa-sun");
            label.textContent = "الوضع الفاتح";
            localStorage.setItem("theme", "dark");
        } else {
            icon.classList.replace("fa-sun", "fa-moon");
            label.textContent = "الوضع الداكن";
            localStorage.setItem("theme", "light");
        }
    });
});