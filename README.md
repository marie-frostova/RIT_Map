# RIT_Map
### Что это такое:
Проект на главной форме содержит карту из библиотеки GMap.NET,
загружает из базы данных Microsoft SQL Server географические координаты условных единиц техники и отображает их на карте в виде маркеров.\
Маркеры можно перемещать с помощью мыши и сохранять новые координаты в базу данных.\
В этом проекте EntityFramework не используется, вместо него T-SQL.
### Как запустить:
Cкачать с помощью NuGet пакет GMap.NET.WinForms.\
В папке проекта есть файл SQLQuery.sql, его можно запустить прямо в MS SQL Server. Он создает и наполняет базу данных, после чего можно запускать само приложение.
### Немного по управлению: 
Клик правой кнопкой мыши по маркеру открывает форму для редактирования (редактируемый маркер становится красным), где можно изменить имя маркера и точно задать координаты. 
Если точность не требуется, маркер можно переместить, если потянуть за него (DragAndDrop). 
Также можно создать маркер с нуля, если нажать на соответствующую кнопку.