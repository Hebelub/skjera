const uri = 'api/eventApi/';

function getEvent(id) {
    return fetch(uri + id)
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error('Unable to get items.', error));
}

function getEvents() {
    return fetch(uri)
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error('Unable to get items.', error));
}

function getEventsBetweenDates(fromDate, toDate) {
    return fetch(uri + "date/" + fromDate + "/" + toDate, { method: 'GET' })
        .then(response => response.json())
        .catch(error => console.error('Unable to get items.', error));
}
