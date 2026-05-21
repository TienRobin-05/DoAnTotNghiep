document.querySelectorAll("[data-cookie-close]").forEach((button) => {
  button.addEventListener("click", () => {
    document.getElementById("cookieBox")?.classList.add("is-hidden");
  });
});
