$(function () {
    // Программное открытие окна выбора файла по щелчку
    $('figure').on('click', function () {
        $(':file').trigger('click');
       
    })

    // При перетаскивании файлов в форму, подсветить
    $('section').on('dragover', function (e) {
        $(this).addClass('dd');
        e.preventDefault();
        e.stopPropagation();
    });

    // Предотвратить действие по умолчанию для события dragenter
    $('section').on('dragenter', function (e) {
        e.preventDefault();
        e.stopPropagation();
    });

    $('section').on('dragleave', function (e) {
        $(this).removeClass('dd');
    });

    $('section').on('drop', function (e) {
        if (e.originalEvent.dataTransfer) {
            if (e.originalEvent.dataTransfer.files.length) {
                e.preventDefault();
                e.stopPropagation();


                upload(e.originalEvent.dataTransfer.files);
            }
        }
    });

    $(':file').on('change', function () {
        upload($(this).prop('files'));
       
    });
    
});

   
function randomString(length) {
    var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz".split("");
    if (!length) {
        length = Math.floor(Math.random() * chars.length);
    }
    var str = "";
    for (var i = 0; i < length; i++) {
        str += chars[Math.floor(Math.random() * chars.length)];
    }
    return str;
}

function delClick(clicked_id) {
    
    var parent = $("#"+clicked_id).parent()
    var a_href = $(parent).find('a').attr('href');
    console.log(a_href);
    $.ajax({
        url: '/Lot/ImageDelete',
        method: 'POST',
        data: { id: a_href },
        success: function (data)
        {
            if (data == "ok") {
                $('.progress').hide();
                parent.empty();
                parent.remove();
            }
        }
    });
    //alert(parent);
    //console.log(ref);
}
// Функция загрузки файлов
function upload(files) {
    // Создаем объект FormData
    var formData = new FormData();

    // Пройти в цикле по всем файлам
    for (var i = 0; i < files.length; i++) {
        // С помощью метода append() добавляем файлы в объект FormData
        formData.append('file_' + i, files[i]);
    }

    // Ajax-запрос на сервер
    $.ajax({
        type: 'POST',
        url: '/Lot/ImageUpload', // URL на метод действия Upload контроллера HomeController
        data: formData,
        processData: false,
        contentType: false,
        beforeSend: function () {
            $('section').removeClass('dd');

            // Перед загрузкой файла удалить старые ошибки и показать индикатор
            $('.error').text('').hide();
            $('.progress').show();

            // Установить прогресс-бар на 0
            $('.progress-bar').css('width', '0');
            $('.progress-value').text('0 %');
        },
        success: function (data) {
            if (data.Error) {
                console.log("fail");
                $('.error').text(data.Error).show();
                $('.progress').hide();
            }
            else {
                console.log("succ");

                $('.progress-bar').css('width', '100%');
                $('.progress-value').text('100 %');

                // Отобразить загруженные картинки
                if (data.Files) {
                    // Обертка для картинки со ссылкой
                    var id = randomString(6);
                    var img = '<div><btn id="' + id + '"' +' onclick="delClick(this.id)" style = "position: absolute; height: 16px; width: 16px; top: 0px; right: 0px; z-index: 10; background-image: url(https://png.icons8.com/small/16/000000/delete-sign.png)" ></btn > <a href="0123" target="_blank"><span style="background-image: url(0123)"></span></a></div > ';
                    //var img = '<a href="0" target="_blank"><span style="background-image: url(0)"></span></a>';

                    for (var i = 0; i < data.Files.length; i++) {
                        // Сгенерировать вставляемый элемент с картинкой
                        // (символ 0 заменяем ссылкой с помощью регулярного выражения)
                        var element = $(img.replace(/0123/g, data.Files[i]));

                        // Добавить в контейнер
                        $('.images').append(element);
                    }
                }
            }
        },
        xhrFields: { // Отслеживаем процесс загрузки файлов
            onprogress: function (e) {
                if (e.lengthComputable) {
                    // Отображение процентов и длины прогресс бара
                    var perc = e.loaded / 100 * e.total;
                    $('.progress-bar').css('width', perc + '%');
                    $('.progress-value').text(perc + ' %');
                }
            }
        },
    });
}