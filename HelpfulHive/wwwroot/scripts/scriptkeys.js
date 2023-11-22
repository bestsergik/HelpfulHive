function insertHtmlAtCursor(element, html) {
    // Проверяем, поддерживается ли execCommand
    if (document.queryCommandSupported('insertHTML')) {
        // Устанавливаем фокус на элемент
        element.focus();
        // Выполняем команду вставки HTML
        document.execCommand('insertHTML', false, html);
    }
}



function executeCommand(element, command, argument) {
    element.focus();
    document.execCommand(command, false, argument || null);
}



// Проверяем, что DOM полностью загружен
document.addEventListener('DOMContentLoaded', function () {

    // Делегирование события клика для '.dropdown-select'
    document.addEventListener('click', function (e) {
        if (e.target.matches('.dropdown-select, .dropdown-select *')) {
            var dropdown = e.target.closest('.dropdown');
            dropdown.classList.toggle('active');
        }
    });

    // Делегирование события клика для '.dropdown-item'
    document.addEventListener('click', function (e) {
        if (e.target.matches('.dropdown-item')) {
            var text = e.target.textContent;
            var dropdown = e.target.closest('.dropdown');
            var dropdownSelect = dropdown.querySelector('.dropdown-select');
            dropdownSelect.querySelector('span').textContent = text;

            // Добавляем класс 'dropdown-select-selected', когда выбран элемент
            dropdownSelect.classList.add('dropdown-select-selected');

            dropdown.classList.remove('active');
            // Здесь необходимо обновить значение, которое используется в Blazor
            // Может потребоваться Blazor JS Interop
        }
    });

});


function animateCheckmark(x, y) {
    var checkmark = document.getElementById('checkmark');
    checkmark.style.left = (x - 25) + 'px';
    checkmark.style.top = (y - 25) + 'px';
    checkmark.style.visibility = 'visible';
    checkmark.style.zIndex = '1000'; // Устанавливаем высокий z-index

    gsap.fromTo(checkmark,
        { scale: 0.5, autoAlpha: 0 },
        { scale: 1, autoAlpha: 1, duration: 0.7, ease: "back.out" });

    gsap.to(checkmark, { scale: 1.2, autoAlpha: 0, delay: 1, duration: 0.5, onComplete: () => checkmark.style.visibility = 'hidden' });
}



function toggleSubTabs(tabId) {
    var allSubTabs = document.querySelectorAll('.modern-sub-tabs');
    var targetSubTab = document.getElementById('sub-tabs-' + tabId);

    if (!targetSubTab) return;

    if (targetSubTab.style.maxHeight) {
        // Если уже открыто, закрываем
        targetSubTab.style.maxHeight = null;
    } else {
        // Если закрыто, то сначала закрываем все, затем открываем целевую вкладку
        for (var i = 0; i < allSubTabs.length; i++) {
            allSubTabs[i].style.maxHeight = null;
        }
        // После того как все вкладки закрыты, открываем нужную
        targetSubTab.style.maxHeight = targetSubTab.scrollHeight + "px";
    }
}
