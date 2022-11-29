!function () {

    let today = moment();

    function Calendar(selector) {
        this.eventColor = 'yellow';
        this.events = [];
        this.el = document.querySelector(selector);
        this.current = moment().date(1);
        this.draw();
        let current = document.querySelector('.today');
        if (current) {
            let self = this;
            window.setTimeout(() => {
                self.openDay(current);
            }, 500);
        }
    }
    
    Calendar.prototype.getEventsThisMonth = async function () {

        let fromDate = moment(this.current).startOf('month').startOf('week').format('YYYY-MM-DD');
        let toDate = moment(this.current).endOf('month').endOf('week').format('YYYY-MM-DD');
        
        const events = await getEventsBetweenDates(fromDate, toDate);

        // Changing fetched events startTime from string to a moment object
        events.forEach((event) => {
            event.startTime = moment(event.startTime, 'YYYY-MM-DDTHH:mm:ss');
        });
        
        return events;
    }

    Calendar.prototype.draw = async function () {
        // Fetch data from API
        this.events = await this.getEventsThisMonth();
        
        //Create Header
        this.drawHeader();

        //Draw Month
        this.drawMonth();

        this.drawLegend();
    }

    Calendar.prototype.drawHeader = function () {
        let self = this;
        if (!this.header) {
            //Create the header elements
            this.header = createElement('div', 'header');
            this.header.className = 'header';

            this.title = createElement('h1');

            let right = createElement('div', 'right');
            right.addEventListener('click', function () {
                self.nextMonth();
            });

            let left = createElement('div', 'left');
            left.addEventListener('click', function () {
                self.prevMonth();
            });

            //Append the Elements
            this.header.appendChild(this.title);
            this.header.appendChild(right);
            this.header.appendChild(left);
            this.el.appendChild(this.header);
        }

        this.title.innerHTML = this.current.format('MMMM YYYY');
    }

    Calendar.prototype.drawMonth = function () {
        let self = this;

        if (this.month) {
            this.oldMonth = this.month;
            this.oldMonth.className = 'month out ' + (self.next ? 'next' : 'prev');
            this.oldMonth.addEventListener('webkitAnimationEnd', () => {
                self.oldMonth.parentNode.removeChild(self.oldMonth);
                self.month = createElement('div', 'month');
                self.backFill();
                self.currentMonth();
                self.forwardFill();
                self.el.appendChild(self.month);
                window.setTimeout(function () {
                    self.month.className = 'month in ' + (self.next ? 'next' : 'prev');
                }, 16);
            });
        } else {
            this.month = createElement('div', 'month');
            this.el.appendChild(this.month);
            this.backFill();
            this.currentMonth();
            this.forwardFill();
            this.month.className = 'month new';
        }
    }

    Calendar.prototype.backFill = function () {
        let clone = this.current.clone();
        let dayOfWeek = clone.day();

        if (!dayOfWeek) {
            return;
        }

        clone.subtract('days', dayOfWeek + 1);

        for (let i = dayOfWeek; i > 0; i--) {
            this.drawDay(clone.add('days', 1));
        }
    }

    Calendar.prototype.forwardFill = function () {
        let clone = this.current.clone().add('months', 1).subtract('days', 1);
        let dayOfWeek = clone.day();

        if (dayOfWeek === 6) {
            return;
        }

        for (let i = dayOfWeek; i < 6; i++) {
            this.drawDay(clone.add('days', 1));
        }
    }

    Calendar.prototype.currentMonth = function () {
        let clone = this.current.clone();

        while (clone.month() === this.current.month()) {
            this.drawDay(clone);
            clone.add('days', 1);
        }
    }

    Calendar.prototype.drawWeek = function (day) {
        if (!this.week || day.day() === 0) {
            this.week = createElement('div', 'week');
            if (this.month !== undefined) {
                this.month.appendChild(this.week);
            }
        }
    }

    Calendar.prototype.drawDay = function (day) {       
        let self = this;
        this.drawWeek(day);

        // Outer Day
        let outer = createElement('div', this.getDayClass(day));
        outer.addEventListener('click', function () {
            self.openDay(this);
        });

        // Day Name
        let name = createElement('div', 'day-name', day.format('ddd'));

        // Day Number
        let number = createElement('div', 'day-number', day.format('DD'));
        
        
        // Day Month 
        let monthNumberElement = createElement('div', 'month-number', day.format('MM'));
        monthNumberElement.style.display = 'none';

        //Events
        let events = createElement('div', 'day-events');
        this.drawEvents(this.getEventsOfDay(day), events);

        outer.appendChild(name);
        outer.appendChild(number);
        outer.appendChild(monthNumberElement);
        outer.appendChild(events);
        this.week.appendChild(outer);
    }
    
    Calendar.prototype.getEventsOfDay = function (day) {
        if (/* day.month() !== this.current.month() ||*/ this.events === undefined)
            return [];
        let eventsOfDay = [];
        
        
        this.events.forEach((event) => {
            if (event.startTime.isSame(day, "day")) {
                eventsOfDay.push(event);
            }
        });
        return eventsOfDay;
    }

    Calendar.prototype.drawEvents = function (events, element) {
        events.forEach(function (ev) {
            let evSpan = createElement('span', 'yellow');
            element.appendChild(evSpan);
        });
    }

    Calendar.prototype.getDayClass = function (day) {
        let classes = ['day'];
        if (day.month() !== this.current.month()) {
            classes.push('other');
        } else if (today.isSame(day, 'day')) {
            classes.push('today');
        }
        return classes.join(' ');
    }

    Calendar.prototype.openDay = function (el) {
        let detailsElement, arrowElement;
        let dayNumber = +el.querySelectorAll('.day-number')[0].innerText || +el.querySelectorAll('.day-number')[0].textContent;
        let monthNumber = (+el.querySelectorAll('.month-number')[0].innerText || +el.querySelectorAll('.month-number')[0].textContent) - 1;
        
        let day = this.current.clone().date(dayNumber).month(monthNumber);
        
        let currentOpened = document.querySelector('.details');

        //Check to see if there is an open detais box on the current row
        if (currentOpened && currentOpened.parentNode === el.parentNode) {
            detailsElement = currentOpened;
            arrowElement = document.querySelector('.arrow');
        } else {
            //Close the open events on differnt week row
            //currentOpened && currentOpened.parentNode.removeChild(currentOpened);
            if (currentOpened) {
                currentOpened.addEventListener('webkitAnimationEnd', function () {
                    currentOpened.parentNode.removeChild(currentOpened);
                });
                currentOpened.addEventListener('oanimationend', function () {
                    currentOpened.parentNode.removeChild(currentOpened);
                });
                currentOpened.addEventListener('msAnimationEnd', function () {
                    currentOpened.parentNode.removeChild(currentOpened);
                });
                currentOpened.addEventListener('animationend', function () {
                    currentOpened.parentNode.removeChild(currentOpened);
                });
                currentOpened.className = 'details out';
            }

            //Create the Details Container
            detailsElement = createElement('div', 'details in');

            //Create the arrow
            arrowElement = createElement('div', 'arrow');

            //Create the event wrapper
            detailsElement.appendChild(arrowElement);
            el.parentNode.appendChild(detailsElement);
        }
        
        this.renderEvents(this.getEventsOfDay(day), detailsElement);

        arrowElement.style.left = el.offsetLeft - el.parentNode.offsetLeft + 27 + 'px';
    }

    // Render events in wrapper of the opened day
    Calendar.prototype.renderEvents = function (events, element) {
        
        //Remove any events in the current details element
        let currentWrapper = element.querySelector('.events');
        let wrapper = createElement('div', 'events in' + (currentWrapper ? ' new' : ''));
        
        events.forEach((ev) => {
            let div = createElement('div', 'event')
            div.classList.add("customCard", "calendarEvent");
            div.onclick = () => { goToEventPage(ev.id); };
            
            let squareEl = createElement('div', 'event-category ' + this.eventColor);
            let titleEl = createElement('span', '', ev.title);
            let timeEl = createElement('span', '', ev.startTime.format('HH:mm'));
            
            div.appendChild(squareEl);
            div.appendChild(titleEl);
            div.appendChild(timeEl);
            wrapper.appendChild(div);
        });

        if (!events.length) {
            let div = createElement('div', 'event empty');
            let span = createElement('span', '', 'No Events');

            div.appendChild(span);
            wrapper.appendChild(div);
        }

        if (currentWrapper) {
            currentWrapper.className = 'events out';
            currentWrapper.addEventListener('webkitAnimationEnd', function () {
                currentWrapper.parentNode.removeChild(currentWrapper);
                element.appendChild(wrapper);
            });
            currentWrapper.addEventListener('oanimationend', function () {
                currentWrapper.parentNode.removeChild(currentWrapper);
                element.appendChild(wrapper);
            });
            currentWrapper.addEventListener('msAnimationEnd', function () {
                currentWrapper.parentNode.removeChild(currentWrapper);
                element.appendChild(wrapper);
            });
            currentWrapper.addEventListener('animationend', function () {
                currentWrapper.parentNode.removeChild(currentWrapper);
                element.appendChild(wrapper);
            });
        } else {
            element.appendChild(wrapper);
        }
    }

    Calendar.prototype.drawLegend = function () {
        let legend = createElement('div', 'legend');
        let calendars = this.events.map(function (e) {
            return e.calendar + '|' + this.eventColor;
        }).reduce(function (memo, e) {
            if (memo.indexOf(e) === -1) {
                memo.push(e);
            }
            return memo;
        }, []).forEach(function (e) {
            let parts = e.split('|');
            let entry = createElement('span', 'entry ' + parts[1], parts[0]);
            legend.appendChild(entry);
        });
        this.el.appendChild(legend);
    }

    Calendar.prototype.nextMonth = function () {
        this.current.add('months', 1);
        this.next = true;
        this.draw();
    }

    Calendar.prototype.prevMonth = function () {
        this.current.subtract('months', 1);
        this.next = false;
        this.draw();
    }

    window.Calendar = Calendar;

    function createElement(tagName, className, innerText) {
        let ele = document.createElement(tagName);
        if (className) {
            ele.className = className;
        }
        if (innerText) {
            ele.innderText = ele.textContent = innerText;
        }
        return ele;
    }
}();

!function () {
    let calendar = new Calendar('#calendar');
}();