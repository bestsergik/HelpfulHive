document.addEventListener('DOMContentLoaded', function () {
    var slider = document.getElementById('slider');
    var isDragging = false;

    slider.addEventListener('mousedown', function () {
        isDragging = true;
    });

    document.addEventListener('mouseup', function () {
        isDragging = false;
    });

    document.addEventListener('mousemove', function (event) {
        if (isDragging) {
            var leftWidth = event.clientX;
            var rightWidth = window.innerWidth - event.clientX - slider.offsetWidth;
            var leftContainer = slider.previousElementSibling;
            var rightContainer = slider.nextElementSibling;

            leftContainer.style.flexGrow = leftWidth;
            rightContainer.style.flexGrow = rightWidth;
        }
    });
});
