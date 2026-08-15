/* =========================================
   STUDYSPHERE AUTHENTICATION UI
   ========================================= */


/* =========================================
   ELEMENTS
   ========================================= */

const signInTab = document.getElementById("signInTab");
const signUpTab = document.getElementById("signUpTab");

const signInForm = document.getElementById("signInForm");
const signUpForm = document.getElementById("signUpForm");

const toggleSlider = document.getElementById("toggleSlider");

const goToSignUp = document.getElementById("goToSignUp");
const goToSignIn = document.getElementById("goToSignIn");

const studentRole = document.getElementById("studentRole");
const instructorRole = document.getElementById("instructorRole");

const instructorDetails =
    document.getElementById("instructorDetails");

const signupPassword =
    document.getElementById("signupPassword");

const confirmPassword =
    document.getElementById("confirmPassword");

const passwordMatch =
    document.getElementById("passwordMatch");

const strengthText =
    document.getElementById("strengthText");

const strengthBars =
    document.querySelectorAll(".strength-bars span");


/* =========================================
   SWITCH TO SIGN IN
   ========================================= */
function showSignIn() {

    signInTab.classList.add("active");
    signUpTab.classList.remove("active");

    toggleSlider.classList.remove("signup");

    signUpForm.classList.remove("active-form");

    setTimeout(() => {

        signInForm.classList.add("active-form");

    }, 80);
}

/* =========================================
   SWITCH TO SIGN UP
   ========================================= */
function showSignUp() {

    signUpTab.classList.add("active");
    signInTab.classList.remove("active");

    toggleSlider.classList.add("signup");

    signInForm.classList.remove("active-form");

    setTimeout(() => {

        signUpForm.classList.add("active-form");

    }, 80);
}


/* =========================================
   TOGGLE BUTTONS
   ========================================= */

signInTab.addEventListener("click", showSignIn);

signUpTab.addEventListener("click", showSignUp);

goToSignUp.addEventListener("click", showSignUp);

goToSignIn.addEventListener("click", showSignIn);


/* =========================================
   PASSWORD SHOW / HIDE
   ========================================= */

document.querySelectorAll(".password-toggle")
    .forEach(button => {

        button.addEventListener("click", function () {

            const targetId = this.dataset.target;

            const passwordInput =
                document.getElementById(targetId);

            const icon =
                this.querySelector("i");


            if (passwordInput.type === "password") {

                passwordInput.type = "text";

                icon.classList.remove("fa-eye");

                icon.classList.add("fa-eye-slash");

                this.setAttribute(
                    "aria-label",
                    "Hide password"
                );

            } else {

                passwordInput.type = "password";

                icon.classList.remove("fa-eye-slash");

                icon.classList.add("fa-eye");

                this.setAttribute(
                    "aria-label",
                    "Show password"
                );
            }

        });

    });


/* =========================================
   ROLE SELECTION
   ========================================= */

studentRole.addEventListener("click", function () {

    studentRole.classList.add("active");

    instructorRole.classList.remove("active");

    instructorDetails.classList.remove("show");

    setInstructorFieldsRequired(false);
});


instructorRole.addEventListener("click", function () {

    instructorRole.classList.add("active");

    studentRole.classList.remove("active");

    instructorDetails.classList.add("show");

    setInstructorFieldsRequired(true);
});


/* =========================================
   INSTRUCTOR REQUIRED FIELDS
   ========================================= */

function setInstructorFieldsRequired(required) {

    const fields = [
        document.getElementById("professionalTitle"),
        document.getElementById("expertise"),
        document.getElementById("qualification")
    ];

    fields.forEach(field => {

        field.required = required;

    });
}


/* =========================================
   PASSWORD STRENGTH
   ========================================= */

signupPassword.addEventListener(
    "input",
    function () {

        const password = this.value;

        let strength = 0;


        if (password.length >= 8) {
            strength++;
        }

        if (/[A-Z]/.test(password)) {
            strength++;
        }

        if (/[0-9]/.test(password)) {
            strength++;
        }

        if (/[^A-Za-z0-9]/.test(password)) {
            strength++;
        }


        strengthBars.forEach((bar, index) => {

            bar.style.background =
                index < strength
                    ? "var(--primary-dark)"
                    : "var(--border)";

        });


        if (password.length === 0) {

            strengthText.textContent =
                "Use 8+ characters";

        } else if (strength === 1) {

            strengthText.textContent =
                "Weak password";

        } else if (strength === 2) {

            strengthText.textContent =
                "Fair password";

        } else if (strength === 3) {

            strengthText.textContent =
                "Good password";

        } else {

            strengthText.textContent =
                "Strong password";

        }


        checkPasswordMatch();

    }
);


/* =========================================
   CONFIRM PASSWORD
   ========================================= */

confirmPassword.addEventListener(
    "input",
    checkPasswordMatch
);


function checkPasswordMatch() {

    const password =
        signupPassword.value;

    const confirm =
        confirmPassword.value;


    if (confirm.length === 0) {

        passwordMatch.textContent = "";

        return;
    }


    if (password === confirm) {

        passwordMatch.textContent =
            "Passwords match";

        passwordMatch.style.color =
            "var(--success)";

    } else {

        passwordMatch.textContent =
            "Passwords do not match";

        passwordMatch.style.color =
            "var(--danger)";
    }
}


/* =========================================
   FORM SUBMISSION
   =========================================
   
   UI ONLY:
   Prevent actual submission for now.
   Later .NET backend will handle this.
   ========================================= */

// signInForm.addEventListener(
//     "submit",
//     function (event) {

//         event.preventDefault();

//         console.log("Sign in UI submitted");

//         /*
//             Later:

//             POST /Account/Login

//             Backend checks:
//             Email
//             Password
//             User Role

//             Then redirects to:

//             Student Dashboard
//             OR
//             Instructor Dashboard
//         */
//     }
// );


signUpForm.addEventListener(
    "submit",
    function (event) {

        event.preventDefault();


        if (
            signupPassword.value !==
            confirmPassword.value
        ) {

            alert("Passwords do not match.");

            return;
        }


        console.log("Sign up UI submitted");

        /*
            Later .NET backend will receive:

            Full Name
            Email
            Phone
            Password
            Role

            If Role = Instructor:

            Professional Title
            Area of Expertise
            Qualification
        */
    }
);