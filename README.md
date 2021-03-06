[![Github All Releases](https://img.shields.io/github/downloads/golubovai/NetworkAdapterRouteControl/total.svg)]()

# NetworkAdapterRouteControl
Controls and sync routing parameters of network adapter (VPN)

Позволяет задавать и производить синхронизацию параметров маршрутизации заданного адаптера сетевого подключения. Может использоваться для сохранения интернет подключения (маршрутизации по умолчанию) на хосте пользователя при активном VPN-подключении в тех случаях, когда клиент VPN-подключения в принудительном порядке изменяет и приоритезирует таблицу маршрутизации.

В настройках можно указать список IP-адресов, которые будут маршрутизироваться через выбранный адаптер VPN-подключения. Все остальные маршруты будут очищаться из таблицы маршрутизации выбранного адаптера. Так же при синхронизации задается метрика выбранного адаптера сетевого подключения.

Работа протестирована с использованием VPN-клиента Check Point Endpoint Security.

