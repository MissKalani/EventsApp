/**
    Prototype for handling events in the list and on the map.
*/

var EventManager = (function () {
    "use strict";

    var EventManager = {};

    EventManager.EVENT_HOSTED = 0;
    EventManager.EVENT_INVITED = 1;
    EventManager.EVENT_PUBLIC = 2;

    /**
        @class Event

        Used to specify an event.
    */
    EventManager.Event = function () {
        this.brief = "";
        this.detailed = "";
        this.address = "";
        this.position = new google.maps.LatLng(0, 0);
        this.userEventRelation = EventManager.EVENT_HOSTED;
    };

    /**
        @class EventListItem

        Represents an item in the event list.
    */
    EventManager.EventListItem = function () {
        this.containerElement = null;
        this.briefElement = null;
        this.detailedElement = null;
    };

    EventManager.EventListItem.prototype.isDetailShown = function () {
        return this.detailedElement.style.display === 'block';
    }

    EventManager.EventListItem.prototype.showDetailed = function () {
        this.detailedElement.setAttribute('style', 'display: block');
    }

    EventManager.EventListItem.prototype.hideDetailed = function () {
        this.detailedElement.setAttribute('style', 'display: none');
    }

    EventManager.EventListItem.prototype.toggleDetailed = function () {
        var isOpen = this.isDetailShown();
        if (isOpen) {
            this.hideDetailed();
        } else {
            this.showDetailed();
        }
    }

    /**
        @class EventEntry

        Represents an internal event entry (holds references to both the marker and the list item).
    */
    EventManager.EventEntry = function () {
        this.event = null;
        this.marker = null;
        this.item = null;
    };

    EventManager.EventEntry.prototype.show = function () {
        this.marker.setVisible(true);
        this.item.containerElement.setAttribute('style', 'display: block');
    }
    
    EventManager.EventEntry.prototype.hide = function () {
        this.marker.setVisible(false);
        this.item.containerElement.setAttribute('style', 'display: none');
    }

    /**
        @class Manager

        Public interface for manipulating events.
    */
    EventManager.Manager = function (listElement, mapObject) {
        this.listElement = listElement;
        this.mapObject = mapObject;
        this.eventEntries = [];
    };

    EventManager.Manager.prototype.addEvent = function (event) {
        var entry = new EventManager.EventEntry();

        entry.event = event;
        entry.marker = this.createEventMarker(event);
        entry.item = this.createEventListItem(event);

        // Setup event listeners.
        var _this = this;
        entry.marker.addListener('click', function () {
            entry.item.showDetailed();
            _this.mapObject.setCenter(event.position);
            _this.mapObject.setZoom(8);
        });

        entry.item.briefElement.addEventListener('click', function (e) {
            entry.item.toggleDetailed();
            if (entry.item.isDetailShown()) {
                _this.mapObject.setCenter(event.position);
                _this.mapObject.setZoom(8);
            }
        });

        this.eventEntries.push(entry);
    };

    EventManager.Manager.prototype.createEventMarker = function (event) {
        var marker = new google.maps.Marker({
            position: event.position,
            map: this.mapObject,
            title: event.brief
        });

        return marker;
    }

    EventManager.Manager.prototype.createEventListItem = function (event) {
        var item = new EventManager.EventListItem();

        item.containerElement = document.createElement('div');
        item.briefElement = document.createElement('div');
        item.detailedElement = document.createElement('div');

        this.listElement.appendChild(item.containerElement);
        item.containerElement.appendChild(item.briefElement);
        item.containerElement.appendChild(item.detailedElement);

        item.briefElement.innerHTML = event.brief;
        item.detailedElement.innerHTML = event.detailed;

        item.detailedElement.setAttribute('style', 'display: none');

        return item;
    }

    EventManager.Manager.prototype.showAll = function () {
        for (var i = 0; i < this.eventEntries.length; i++) {
            var entry = this.eventEntries[i];
            entry.show();
        }
    }

    EventManager.Manager.prototype.showHosted = function () {
        for (var i = 0; i < this.eventEntries.length; i++) {
            var entry = this.eventEntries[i];
            if (this.eventEntries[i].event.userEventRelation == EventManager.EVENT_HOSTED) {
                entry.show();
            } else {
                entry.hide();
            }
        }
    }

    EventManager.Manager.prototype.showInvited = function () {
        for (var i = 0; i < this.eventEntries.length; i++) {
            var entry = this.eventEntries[i];
            if (this.eventEntries[i].event.userEventRelation == EventManager.EVENT_INVITED) {
                entry.show();
            } else {
                entry.hide();
            }
        }
    }

    EventManager.Manager.prototype.showPublic = function () {
        for (var i = 0; i < this.eventEntries.length; i++) {
            var entry = this.eventEntries[i];
            if (this.eventEntries[i].event.userEventRelation == EventManager.EVENT_PUBLIC) {
                entry.show();
            } else {
                entry.hide();
            }
        }
    }

    return EventManager;
})();


/*
var EVENT_HOSTED = 0;
var EVENT_INVITED = 1;
var EVENT_PUBLIC = 2;

function Event() {
    this.brief = "";
    this.detailed = "";
    this.address = "";
    this.position = new google.maps.LatLng(0, 0);
    this.userEventRelation = EVENT_HOSTED;
}

function ListItem() {
    this.containerElement = null;
    this.briefElement = null;
    this.detailedElement = null;
}

function showAll(listElement) {

}

function addEvent(listElement, map, event) {
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
*/