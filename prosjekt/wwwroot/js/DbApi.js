﻿const eventUri = 'api/eventApi/';
const organizationUri = 'api/organizationApi/';

function getEvent(id) {
    return fetch(eventUri + id)
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error('Unable to get items.', error));
}

function getEvents() {
    return fetch(eventUri)
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error('Unable to get items.', error));
}

function getEventsBetweenDates(fromDate, toDate) {
    return fetch(eventUri + "date/" + fromDate + "/" + toDate, { method: 'GET' })
        .then(response => response.json())
        .catch(error => console.error('Unable to get items.', error));
}

// id: int, follow: bool
function followOrganization(id, follow) {
    console.log("Follow", id, follow);
    console.log("Stringify follow", JSON.stringify(follow));
    fetch(organizationUri + "follow/" + id, { 
        method: 'PUT',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(follow)
    }).then(res => {
        console.log("Request complete! response:", res);
    });
}

// id: int, Attend: bool
function attendEvent(id, attend) {
    fetch(eventUri + "attend/" + id + "/" + attend, { method: 'POST' })
}