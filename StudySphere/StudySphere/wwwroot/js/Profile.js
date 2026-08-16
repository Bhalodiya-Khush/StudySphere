document.addEventListener("DOMContentLoaded", function () {

    const editButton = document.getElementById("editProfileBtn");

    if (editButton) {
        editButton.addEventListener("click", function () {

            editButton.innerHTML =
                '<i class="fa-solid fa-check"></i> Editing';

            editButton.style.pointerEvents = "none";

            setTimeout(() => {

                editButton.innerHTML =
                    '<i class="fa-solid fa-pen"></i> Edit Profile';

                editButton.style.pointerEvents = "auto";

            }, 1500);
        });
    }


    // Smooth hover animation for cards
    const cards = document.querySelectorAll(
        ".profile-card, .stat-card"
    );

    cards.forEach(card => {

        card.addEventListener("mouseenter", () => {
            card.style.transition =
                "transform .25s ease, box-shadow .25s ease";
        });

    });

});