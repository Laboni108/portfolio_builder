document.addEventListener("DOMContentLoaded", function () {
  var navbar = document.getElementById("navbar");
  var scrollTop = document.getElementById("scrollTop");
  var hamburger = document.getElementById("hamburger");
  var navMobile = document.getElementById("navMobile");

  // Navbar background + scroll-to-top button
  window.addEventListener("scroll", function () {
    if (navbar) navbar.classList.toggle("scrolled", window.scrollY > 50);
    if (scrollTop) scrollTop.classList.toggle("visible", window.scrollY > 400);
  });

  // Hamburger menu
  function closeMobile() {
    if (hamburger) hamburger.classList.remove("open");
    if (navMobile) navMobile.classList.remove("open");
  }

  if (hamburger && navMobile) {
    hamburger.addEventListener("click", function () {
      hamburger.classList.toggle("open");
      navMobile.classList.toggle("open");
    });

    // Close the mobile menu when any link inside it is clicked
    navMobile.querySelectorAll("a").forEach(function (link) {
      link.addEventListener("click", closeMobile);
    });
  }

  // Scroll reveal animation
  var reveals = document.querySelectorAll(".reveal");
  var observer = new IntersectionObserver(function (entries) {
    entries.forEach(function (entry, i) {
      if (entry.isIntersecting) {
        setTimeout(function () {
          entry.target.classList.add("visible");
        }, i * 80);
      }
    });
  }, { threshold: 0.1 });

  reveals.forEach(function (el) { observer.observe(el); });
});
