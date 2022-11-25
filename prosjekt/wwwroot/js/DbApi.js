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
    fetch(uri)
        .then(response => response.json())
        .then(data => console.log(data))
        .catch(error => console.error('Unable to get items.', error));
}
