const navigationToggle = document.querySelector(".nav-menu-toggle");
const mobileNavigation = document.getElementById("mobileNavigation");

if (navigationToggle && mobileNavigation) {
    navigationToggle.addEventListener("click", () => {
        const isOpen = mobileNavigation.classList.toggle("is-open");
        navigationToggle.setAttribute("aria-expanded", String(isOpen));
        navigationToggle.setAttribute(
            "aria-label",
            isOpen ? "Close navigation" : "Open navigation");
    });
}
