# adminHMD

**Multipurpose Bootstrap 5 Admin Dashboard Template**

---

## 🚀 Overview

**adminHMD** is a clean, modern, and fully responsive admin dashboard template built with **HTML5, CSS3, and Bootstrap 5 (local assets)**.
It is designed for developers who want a fast, scalable, and customizable UI for admin panels, SaaS apps, CRM systems, and more.

---

## ✨ Key Features

* ✔ Bootstrap 5 (No CDN, fully local setup)
* ✔ Fully responsive (mobile-first)
* ✔ Collapsible sidebar (mini mode supported)
* ✔ Modern UI with cards, shadows, spacing system
* ✔ Charts integration (Chart.js - local)
* ✔ Data tables (search, pagination ready)
* ✔ Authentication pages (login, register, forgot password)
* ✔ Clean & reusable code structure
* ✔ Cross-browser compatible
* ✔ Developer-friendly file structure
* ✔ Light + Dark mode (optional)
* ✔ No console errors

---

## 📁 Project Structure

```
adminHMD/
│
├── html/
│   ├── index.html
│   ├── users.html
│   ├── profile.html
│   ├── settings.html
│   ├── login.html
│   ├── register.html
│   ├── charts.html
│   ├── tables.html
│   ├── 404.html
│
├── assets/
│   ├── css/
│   │   ├── bootstrap.min.css
│   │   ├── style.css
│   │
│   ├── js/
│   │   ├── bootstrap.bundle.min.js
│   │   ├── main.js
│   │
│   ├── scss/ (optional)
│   ├── images/
│   ├── vendors/
│   │   ├── bootstrap-icons/
│   │   ├── chartjs/
│   │
│
├── documentation/
│   └── index.html
│
└── preview-image/
```

---

## ⚙️ Installation Guide

1. Download the project ZIP file
2. Extract the file
3. Open the project folder
4. Navigate to:

```
/html/index.html
```

5. Open it in your browser or use Live Server

✅ No build tools required
✅ No npm installation needed

---

## 🔧 Customization Guide

### 🎨 Change Colors

Edit:

```
assets/css/style.css
```

Example:

```css
:root {
  --primary-color: #4e73df;
}
```

---

### 🧩 Modify Sidebar Menu

Edit:

```
html/index.html
```

Example:

```html
<li class="nav-item">
  <a href="users.html" class="nav-link">Users</a>
</li>
```

---

### 📊 Add Charts

Use local Chart.js from:

```
assets/vendors/chartjs/
```

Example:

```javascript
new Chart(document.getElementById("chart"), {
  type: 'bar',
  data: { labels: ['Jan','Feb'], datasets: [{ data: [10,20] }] }
});
```

---

### 🌙 Enable Dark Mode

Add class:

```html
<body class="dark">
```

Customize in:

```
assets/css/style.css
```

---

## 📱 Responsive Design

adminHMD is fully responsive and tested on:

* Mobile devices
* Tablets
* Desktop screens

---

## 🌐 Browser Support

* Chrome ✔
* Firefox ✔
* Edge ✔
* Safari ✔

---

## 📦 Dependencies

* Bootstrap 5 (local)
* Chart.js (local)
* Bootstrap Icons (local)

---

## ⚠️ Notes

* Do not use CDN for production (ThemeForest requirement)
* Keep file structure unchanged
* Optimize images before publishing

---

## 🧠 Developer Guidelines

* Follow clean code structure
* Avoid inline CSS/JS
* Use reusable components
* Maintain consistent naming conventions

---

## 📄 Documentation

Detailed documentation available in:

```
/documentation/index.html
```

---

## 🖼️ Preview

Add preview image in:

```
/preview-image/preview.jpg
```

---

## 🧑‍💻 Author

**Hasan Mahmud Dev**

---

## 📜 License

This template is for personal and commercial use under proper licensing.

---

## ⭐ Support

If you like this template, consider giving a ⭐ on GitHub or rating on ThemeForest.

---

## 🔥 Future Updates

* More UI components
* Advanced charts
* RTL support
* Multiple layouts

---
