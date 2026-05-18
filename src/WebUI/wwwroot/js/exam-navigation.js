"use strict";

(function () {
  var form = document.querySelector("[data-exam-form]");
  var panels = document.querySelectorAll("[data-question-panel]");
  var prevButton = document.querySelector("[data-question-prev]");
  var nextButton = document.querySelector("[data-question-next]");
  var indicator = document.querySelector("[data-question-indicator]");

  if (!form || panels.length === 0) {
    return;
  }

  var currentIndex = 0;

  function showQuestion(index) {
    currentIndex = Math.max(0, Math.min(index, panels.length - 1));
    panels.forEach(function (panel, panelIndex) {
      panel.classList.toggle("d-none", panelIndex !== currentIndex);
    });

    if (indicator) {
      indicator.textContent = "Question " + (currentIndex + 1) + " of " + panels.length;
    }

    if (prevButton) {
      prevButton.disabled = currentIndex === 0;
    }

    if (nextButton) {
      nextButton.classList.toggle("d-none", currentIndex === panels.length - 1);
    }
  }

  if (prevButton) {
    prevButton.addEventListener("click", function () {
      showQuestion(currentIndex - 1);
    });
  }

  if (nextButton) {
    nextButton.addEventListener("click", function () {
      showQuestion(currentIndex + 1);
    });
  }

  showQuestion(0);
})();
