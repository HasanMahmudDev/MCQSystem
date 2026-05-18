"use strict";

(function () {
  var timer = document.querySelector("[data-exam-timer]");
  var form = document.querySelector("[data-exam-form]");

  if (!timer || !form) {
    return;
  }

  var secondsRemaining = Number(timer.getAttribute("data-seconds") || "0");

  function render() {
    var minutes = Math.floor(secondsRemaining / 60);
    var seconds = secondsRemaining % 60;
    timer.textContent = String(minutes).padStart(2, "0") + ":" + String(seconds).padStart(2, "0");

    if (secondsRemaining <= 0) {
      form.submit();
      return;
    }

    secondsRemaining -= 1;
    window.setTimeout(render, 1000);
  }

  render();
})();
