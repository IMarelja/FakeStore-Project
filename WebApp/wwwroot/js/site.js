// Password visibility toggle for auth and user forms.
document.addEventListener("DOMContentLoaded", function () {
    const toggleButtons = document.querySelectorAll("[data-password-toggle]");

    toggleButtons.forEach(function (button) {
        const group = button.closest(".input-group");
        const passwordInput = group ? group.querySelector("[data-password-input]") : null;

        if (!passwordInput) {
            return;
        }

        const showText = button.getAttribute("data-show-text") || "Show";
        const hideText = button.getAttribute("data-hide-text") || "Hide";

        button.addEventListener("click", function () {
            const shouldShow = passwordInput.type === "password";
            passwordInput.type = shouldShow ? "text" : "password";
            button.textContent = shouldShow ? hideText : showText;
            button.setAttribute("aria-pressed", shouldShow ? "true" : "false");
            button.setAttribute("aria-label", shouldShow ? "Hide password" : "Show password");
        });
    });
});
