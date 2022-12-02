function getHomePageCheckboxSelection() {
    
    console.log(document.getElementById("homePageCheckboxAttending"));
    
    let attendingChecked = document.getElementById("homePageCheckboxAttending").checked;
    let organizationsChecked = document.getElementById("homePageCheckboxOrganizationsYouFollow").checked;
    let otherChecked = document.getElementById("homePageCheckboxOther").checked;
    
    console.log("isChecked", attendingChecked, organizationsChecked, otherChecked);
}

function getFirstAndLastDateOfCalendar() {
    
}

function fetchHomePageData() {
    getFirstAndLastDateOfCalendar();
}

function drawHomePage() {
    var siteData = fetchSiteData();
    getHomePageCheckboxSelection();
}