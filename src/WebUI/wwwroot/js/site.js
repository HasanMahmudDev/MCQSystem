"use strict";

(function () {
  function confirmAction(message) {
    if (window.Swal) {
      return window.Swal.fire({
        title: "Are you sure?",
        text: message,
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, continue",
        cancelButtonText: "Cancel",
        confirmButtonColor: "#0d6efd",
        cancelButtonColor: "#6c757d"
      }).then(function (result) {
        return result.isConfirmed;
      });
    }

    return Promise.resolve(window.confirm(message));
  }

  var forms = document.querySelectorAll("[data-confirm]");
  Array.prototype.forEach.call(forms, function (form) {
    form.addEventListener("submit", function (event) {
      var message = form.getAttribute("data-confirm");
      if (!message) {
        return;
      }

      event.preventDefault();
      confirmAction(message).then(function (confirmed) {
        if (confirmed) {
          form.submit();
        }
      });
    });
  });

  var toastMessage = document.body.getAttribute("data-toast");
  if (toastMessage && window.Swal) {
    window.Swal.fire({
      toast: true,
      position: "top-end",
      icon: "success",
      title: toastMessage,
      showConfirmButton: false,
      timer: 3200,
      timerProgressBar: true
    });
  }
})();
