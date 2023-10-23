
window.sliderFunctions = {
    initializeSlider: function () {
        const slider = document.getElementById('slider');
        const panelLeft = document.getElementById('panel-left');
        const panelRight = document.getElementById('panel-right');
        const container = document.getElementById('split-container');

        slider.addEventListener('mousedown', function (e) {
            e.preventDefault();

            const startX = e.clientX;
            const startLeft = slider.offsetLeft;

            function onMouseMove(e) {
                let newLeft = startLeft + e.clientX - startX;

                if (newLeft < 0) {
                    newLeft = 0;
                } else if (newLeft > container.offsetWidth - slider.offsetWidth) {
                    newLeft = container.offsetWidth - slider.offsetWidth;
                }

                panelLeft.style.width = `${newLeft}px`;
                panelRight.style.width = `${container.offsetWidth - newLeft - slider.offsetWidth}px`;
                slider.style.left = `${newLeft}px`;
            }

            function onMouseUp() {
                document.removeEventListener('mousemove', onMouseMove);
                document.removeEventListener('mouseup', onMouseUp);
            }

            document.addEventListener('mousemove', onMouseMove);
            document.addEventListener('mouseup', onMouseUp);
        });
    }
};