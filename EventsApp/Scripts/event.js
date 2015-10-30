var map;
function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: 59.690815, lng: 15.215400 },
        zoom: 8
    });
}

function Event() {
    this.brief = "";
    this.detailed = "";
    this.address = "";
    this.position = new google.maps.LatLng(0, 0);
}

function ListItem() {
    this.containerElement = null;
    this.briefElement = null;
    this.detailedElement = null;
}

function addEvent(listElement, event) {
    var marker = new google.maps.Marker({
        position: event.position,
        map: map,
        title: event.brief
    });

    var item = addEventListItem(listElement, event);

    // Setup event listeners.
    marker.addListener('click', function () {
        toggleListItem(item);
        map.setCenter(event.position);
        map.setZoom(8);
    });

    item.briefElement.addEventListener('click', function (e) {
        toggleListItem(item);
        if (isListItemOpen(item)) {
            map.setCenter(event.position);
            map.setZoom(8);
        }
    });
}

function isListItemOpen(item) {
    return item.detailedElement.style.display === 'block';
}

function toggleListItem(item) {
    var isOpen = isListItemOpen(item);
    if (isOpen) {
        item.detailedElement.style.display = 'none';
    } else {
        item.detailedElement.style.display = 'block';
    }
}

function addEventListItem(listElement, event) {
    var item = new ListItem();

    item.containerElement = document.createElement('div');
    item.briefElement = document.createElement('div');
    item.detailedElement = document.createElement('div');

    listElement.appendChild(item.containerElement);
    item.containerElement.appendChild(item.briefElement);
    item.containerElement.appendChild(item.detailedElement);

    item.briefElement.innerHTML = event.brief;
    item.detailedElement.innerHTML = event.detailed;

    item.detailedElement.setAttribute('style', 'display: none');

    return item;
}
