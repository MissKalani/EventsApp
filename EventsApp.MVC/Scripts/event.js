/**
    Prototype for handling events in the list and on the map.
*/

var EventManager = (function () {
    "use strict";

    var EventManager = {};

    EventManager.VISIBILITY_PUBLIC = 0;
    EventManager.VISIBILITY_PRIVATE = 1;

    EventManager.RELATION_HOSTED = 0;
    EventManager.RELATION_INVITED = 1;
    EventManager.RELATION_PUBLIC = 2;

    /**
        @class Event

        Used to specify an event.
    */
    EventManager.Event = function () {
        this.id = 0;
        this.brief = "";
        this.detailed = "";
        this.hostName = "John Doe";
        this.address = "";
        this.position = new google.maps.LatLng(0, 0);
        this.startTime = new Date();
        this.visibility = EventManager.VISIBILITY_PUBLIC;
        this.userEventRelation = EventManager.RELATION_HOSTED;
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
        this.briefElement.className = "event-list-brief expanded";
    }

    EventManager.EventListItem.prototype.hideDetailed = function () {
        this.detailedElement.setAttribute('style', 'display: none');
        this.briefElement.className = "event-list-brief";
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

        // Create the container divs.
        item.containerElement = document.createElement('div');
        item.briefElement = document.createElement('div');
        item.detailedElement = document.createElement('div');
        
        item.briefElement.className = "event-list-brief";
        item.detailedElement.className = "event-list-details";

        this.listElement.appendChild(item.containerElement);
        item.containerElement.appendChild(item.briefElement);
        item.containerElement.appendChild(item.detailedElement);

        // Setup the contents of the brief container.
        var briefParagraph = document.createElement('span');
        briefParagraph.innerHTML = event.brief;
        briefParagraph.className = "brief";

        var hostedByParagraph = document.createElement('span');
        hostedByParagraph.innerHTML = " hosted by " + event.hostName;
        hostedByParagraph.className = "hosted-by";

        item.briefElement.appendChild(briefParagraph);
        item.briefElement.appendChild(hostedByParagraph);

        // Setup the contents of the details container.
        var detailsParagraph = document.createElement('p');
        detailsParagraph.innerHTML = event.detailed;
        detailsParagraph.className = "details";
        item.detailedElement.appendChild(detailsParagraph);

        if (event.address != "")
        {
            var addressParagraph = document.createElement('div');
            addressParagraph.innerHTML = event.address;
            addressParagraph.className = "address";
            item.detailedElement.appendChild(addressParagraph);
        }

        var startTimeParagraph = document.createElement('div');
        startTimeParagraph.innerHTML = event.startTime.getFullYear() + "-"
                                     + (event.startTime.getMonth() + 1) + "-"
                                     + event.startTime.getDate();
        startTimeParagraph.className = "datetime";
        item.detailedElement.appendChild(startTimeParagraph);

        var detailsButton = document.createElement('a');
        detailsButton.innerHTML = "Details";
        detailsButton.className = "btn btn-primary";
        detailsButton.href = "Event/Details/" + event.id;
        item.detailedElement.appendChild(detailsButton);

        // Per default: hide the detailed container.
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
            if (this.eventEntries[i].event.userEventRelation == EventManager.RELATION_HOSTED) {
                entry.show();
            } else {
                entry.hide();
            }
        }
    }

    EventManager.Manager.prototype.showInvited = function () {
        for (var i = 0; i < this.eventEntries.length; i++) {
            var entry = this.eventEntries[i];
            if (this.eventEntries[i].event.userEventRelation == EventManager.RELATION_INVITED) {
                entry.show();
            } else {
                entry.hide();
            }
        }
    }

    EventManager.Manager.prototype.showPublic = function () {
        for (var i = 0; i < this.eventEntries.length; i++) {
            var entry = this.eventEntries[i];
            if (this.eventEntries[i].event.visibility == EventManager.VISIBILITY_PUBLIC) {
                entry.show();
            } else {
                entry.hide();
            }
        }
    }

    return EventManager;
})();