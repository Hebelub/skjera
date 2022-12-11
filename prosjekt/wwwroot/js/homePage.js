
let _calendar = new Calendar('#calendar');
let _fetchedEvents = [];


function getHomePageCheckboxSelection() {
    return {
        attending: document.getElementById("homePageCheckboxAttending").checked, 
        followOrganizer: document.getElementById("homePageCheckboxOrganizationsYouFollow").checked, 
        pastEvents: document.getElementById("homePageCheckboxPast").checked,
        other: document.getElementById("homePageCheckboxOther").checked
    };
}


function eventToJsEvent(events) {
    events.forEach((event) => {
        event.startTime = moment(event.startTime, 'YYYY-MM-DDTHH:mm:ss');
        event.endTime = moment(event.endTime, 'YYYY-MM-DDTHH:mm:ss');
        
        if (event.endTime.isAfter(moment())) {
            if (event.startTime.isBefore(moment())) {
                event.color = 'red';
                event.calendar = 'Active Event';
            }
            else if (event.isUserAttending) {
                event.color = 'blue';
                event.calendar = 'Attending';
            }
            else if (event.isUserFollowingOrganizer) {
                event.color = 'green';
                event.calendar = 'Following';
            }
            else {
                event.color = 'yellow';
                event.calendar = 'Other Events';
            }
        }
        else {
            event.color = 'gray';
            event.calendar = 'Ended';
        }
    });
    
    return events;
}


async function fetchEvents() {
    let events = await getEventsBetweenDates(
        _calendar.getFirstDateOfCalendar(), 
        _calendar.getLastDateOfCalendar()
    );
    
    return events;
}


function hasEventEnded(event) {
    return event.endTime.isBefore(moment());
}


function includeOnlyCheckedEvents(events) {
    let filter = getHomePageCheckboxSelection();
    
    let filteredEvents = [];
    
    events.forEach((event) => {
        if (!filter.pastEvents && hasEventEnded(event)) {
            return;
        }
        
        let other = filter.other && !event.isUserFollowingOrganizer && !event.isUserAttending;
        let followOrganizer = filter.followOrganizer && event.isUserFollowingOrganizer
        let attending = filter.attending && event.isUserAttending;
        
        if (other || followOrganizer || attending) {
            filteredEvents.push(event);
        }        
    })
    
    return filteredEvents;
}


async function updateHomePage(switchMonth=false) {
    if (switchMonth)
        _fetchedEvents = await fetchEvents();
    _fetchedEvents = eventToJsEvent(_fetchedEvents);
    
    _calendar.events = includeOnlyCheckedEvents(_fetchedEvents);
    
    _calendar.draw(switchMonth);
}


function setAttendEvent(id, attend) {
    _fetchedEvents.find((event) => event.id === id).isUserAttending = attend;
    updateHomePage();
}


updateHomePage(true);
