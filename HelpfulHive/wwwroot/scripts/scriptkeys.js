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

function closeModal() {
    var modal = document.querySelector('.add-record-modal');
    // Добавляем класс анимации исчезновения
    modal.classList.add('fadeOutModalAnimation');
    // Удаляем модальное окно после завершения анимации
    modal.addEventListener('animationend', function () {
        modal.style.display = 'none';
        modal.classList.remove('fadeOutModalAnimation'); // очищаем класс для следующего использования
    }, { once: true });
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
            dropdown.querySelector('.dropdown-select span').textContent = text;
            dropdown.classList.remove('active');
            // Здесь необходимо обновить значение, которое используется в Blazor
            // Может потребоваться Blazor JS Interop
        }
    });

});
