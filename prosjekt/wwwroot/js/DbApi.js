const eventUri = 'api/eventApi/';
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
    fetch(organizationUri + "follow/" + id, { 
        method: 'PUT',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(follow)
    }).then(res => {
        return res;
    });
}

// id: int, Attend: bool
    function attendEvent(id, attend) {
    fetch(eventUri + "attend/" + id, { 
        method: 'PUT',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify(attend)
    }).then(res => {
        return res;
    });
}
