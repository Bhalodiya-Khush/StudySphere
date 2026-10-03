const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const test = require("node:test");
const vm = require("node:vm");

const scriptPath = path.resolve(
    __dirname,
    "../../StudySphere/StudySphere/wwwroot/js/studysphere-ui.js");
const script = fs.readFileSync(scriptPath, "utf8");

function createClassList() {
    const classes = new Set();
    return {
        toggle(name) {
            if (classes.has(name)) {
                classes.delete(name);
                return false;
            }
            classes.add(name);
            return true;
        },
        contains(name) {
            return classes.has(name);
        }
    };
}

function createElement() {
    const attributes = new Map();
    const listeners = new Map();
    return {
        classList: createClassList(),
        addEventListener(name, callback) {
            listeners.set(name, callback);
        },
        setAttribute(name, value) {
            attributes.set(name, value);
        },
        getAttribute(name) {
            return attributes.get(name);
        },
        click(name = "click") {
            listeners.get(name)?.();
        }
    };
}

test("mobile navigation toggles open and closed accessibly", () => {
    const toggle = createElement();
    const navigation = createElement();
    const document = {
        querySelector(selector) {
            return selector === ".nav-menu-toggle" ? toggle : null;
        },
        getElementById(id) {
            return id === "mobileNavigation" ? navigation : null;
        }
    };

    vm.runInNewContext(script, { document });

    toggle.click();
    assert.equal(navigation.classList.contains("is-open"), true);
    assert.equal(toggle.getAttribute("aria-expanded"), "true");
    assert.equal(toggle.getAttribute("aria-label"), "Close navigation");

    toggle.click();
    assert.equal(navigation.classList.contains("is-open"), false);
    assert.equal(toggle.getAttribute("aria-expanded"), "false");
    assert.equal(toggle.getAttribute("aria-label"), "Open navigation");
});

test("navigation script safely handles pages without the mobile menu", () => {
    const toggle = createElement();
    const document = {
        querySelector() {
            return toggle;
        },
        getElementById() {
            return null;
        }
    };

    assert.doesNotThrow(() => vm.runInNewContext(script, { document }));
});
