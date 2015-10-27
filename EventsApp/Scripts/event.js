var map;
function initMap() {
    map = new google.maps.Map(document.getElementById('map'), {
        center: { lat: -34.397, lng: 150.644 },
        zoom: 8
    });
}

function Event() {
    this.brief = "";
    this.detailed = "";
    this.address = "";
}

function addEvent(listElement, mapElement, event) {

}

function addEventListItem(listElement, brief, detailed) {
    var item = document.createElement('div');
    var briefItem = document.createElement('div');
    var detailedItem = document.createElement('div');

    listElement.appendChild(item);
    item.appendChild(briefItem);
    item.appendChild(detailedItem);

    briefItem.innerHTML = brief;
    detailedItem.innerHTML = detailed;

    detailedItem.setAttribute('style', 'display: none');
    
    briefItem.addEventListener('click', function (e) {
        var isOpen = detailedItem.style.display === 'block';
        if (isOpen) {
            detailedItem.style.display = 'none';
        } else {
            detailedItem.style.display = 'block';
        }
    });
}

event = new Event();
event.brief = "Hej";
event.detailed = "Test";

addEventListItem(document.getElementById('eventList'), 'Konstig utställning hos Harry', 'Harry bjuder på konstig konst.');
addEventListItem(document.getElementById('eventList'), 'Viktors Bar Mitzva', 'Mazel tow!');