window.addEventListener("scroll", () => {
    const navbar = document.querySelector(".site-navbar");
    if (window.scrollY > 30) navbar.classList.add("scrolled");
    else navbar.classList.remove("scrolled");
});

document.querySelectorAll('a[href^="#"]').forEach(link => {
    link.addEventListener("click", e => {
        const target = document.querySelector(link.getAttribute("href"));
        if (target) {
            e.preventDefault();
            target.scrollIntoView({ behavior: "smooth" });
        }
    });
});

const backToTop = document.createElement("button");
backToTop.textContent = "↑";
backToTop.id = "backToTopBtn";
document.body.appendChild(backToTop);

backToTop.style.cssText = `
    position:fixed;
    right:20px;
    bottom:20px;
    width:45px;
    height:45px;
    border:none;
    border-radius:50%;
    font-size:22px;
    cursor:pointer;
    display:none;
    z-index:2000;
`;

window.addEventListener("scroll", () => {
    backToTop.style.display = window.scrollY > 300 ? "block" : "none";
});

backToTop.addEventListener("click", () => {
    window.scrollTo({ top: 0, behavior: "smooth" });
});

const observer = new IntersectionObserver(entries => {
    entries.forEach(entry => {
        if (entry.isIntersecting) entry.target.classList.add("fade-in");
    });
});

document.querySelectorAll(".feature-card, .card-product, .testimonial")
    .forEach(el => observer.observe(el));
