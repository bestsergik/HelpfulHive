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


