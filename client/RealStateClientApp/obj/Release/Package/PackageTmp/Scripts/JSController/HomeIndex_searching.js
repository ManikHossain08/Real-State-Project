
function initMap() {
    var url = 'http://10.1.15.27/GoogleSvc/AdministrationService.svc/MapObjects';
    $.ajax({
        type: 'GET',
        url: url,
        success: function (savedMapObjects, type, xhr) {
            populateMapObkect(savedMapObjects);
            var map = new google.maps.Map(document.getElementById('map'), {
                zoom: 13,
                center: { lat: 23.777706520371947, lng: 90.34332275390625 }
            });
            var directionsService = new google.maps.DirectionsService;
            var directionsDisplay = new google.maps.DirectionsRenderer({
                draggable: true,
                map: map,
                panel: document.getElementById('right-panel')
            });
            directionsDisplay.setMap(map);
            document.getElementById('submit').addEventListener('click', function () {
                calculateAndDisplayRoute(directionsService, directionsDisplay);
            });
            function calculateAndDisplayRoute(directionsService, directionsDisplay) {
                var waypts = [];
                displayRoute(directionsService, directionsDisplay, waypts);
            }
            directionsDisplay.addListener('directions_changed', function () {
                computeTotalDistance(directionsDisplay.getDirections());
            });
        },
        error: function (xhr) {
            window.alert('error: ' + xhr.statusText);
        }
    });
}

function populateMapObkect(datas) {

    var start = document.getElementById('start');
    for (var i = 0; i < datas.length; i++) {
        var optstart = document.createElement('option');
        optstart.innerHTML = datas[i].Title;
        optstart.value = datas[i].placeId;
        start.appendChild(optstart);
    }
    var end = document.getElementById('end');
    for (var i = 0; i < datas.length; i++) {
        var optend = document.createElement('option');
        optend.innerHTML = datas[i].Title;
        optend.value = datas[i].placeId;
        end.appendChild(optend);
    }

}

function displayRoute(service, display, waypts) {


    service.route({


        origin: {
            placeId: document.getElementById('start').value //"ChIJ-43XxIvEVTcRDc0XGvhvQN0"
        },
        destination: {
            placeId: document.getElementById('end').value //"ChIJqXhA3XboVTcRZ2vOdh0BlnE"
        },
        waypoints: waypts,
        //waypoints: [{
        //    stopover: true,
        //    location: {
        //        placeId: "ChIJ_0p5yV3CVTcRjNlMiAFUS74"//,"ChIJGTzsZozCVTcRx8_q2U-B6tc"
        //    }
        //}],
        //travelMode: 'DRIVING',
        travelMode: document.getElementById('travelBy').value,
        avoidTolls: true
    }, function (response, status) {
        if (status === 'OK') {
            display.setDirections(response);
        } else {
            alert('Could not display directions due to: ' + status);
        }
    });
}

function computeTotalDistance(result) {
    var total = 0;
    var tTime = 0;
    var myroute = result.routes[0];
    for (var i = 0; i < myroute.legs.length; i++) {
        total += myroute.legs[i].distance.value;
        tTime += myroute.legs[i].duration.value;
    }
    total = total / 1000;
    tTime = tTime / 60;
    document.getElementById('total').innerHTML = total + ' km';
    document.getElementById('totalTime').innerHTML = 'About ' + tTime.toFixed(2) + ' mins';
}


function SaveMarkerCoOrdinate() {
    var lat = document.getElementById('Lattitude').value;
    var long = document.getElementById('Longitude').value;
    var objName = document.getElementById('ObjName').value;
    var Note = document.getElementById('Note').value;
    if (confirm('Are you want to save this object ?' + lat + ',' + long) == true) {
        var url = 'http://maps.googleapis.com/maps/api/geocode/json?latlng=' + lat + ',' + long + '&sensor=true'
        $.ajax({
            type: 'GET',
            url: url,
            dataType: 'json',
            success: function (response, type, xhr) {
                placeId = response.results[0].place_id;
                ////////////////////
                var url = 'http://10.1.15.27/GoogleSvc/AdministrationService.svc/createMapObject?objectName=' + objName + '&lat=' + lat + '&longitude=' + long;
                $.ajax({
                    type: 'GET',
                    url: url,
                    dataType: 'json',
                    //data: 'a=a&b=b',
                    success: function (response, type, xhr) {
                        window.alert('Succesfully Saved.');
                    },
                    error: function (xhr) {
                        window.alert('error: ' + xhr.statusText);
                    }
                });
                //////////////

            },
            error: function (xhr) {
                window.alert('error: ' + xhr.statusText);
            }
        });
    }

}

function getlattitue(indicatior) {
    var latLong = 0;
    var url = 'http://ip-api.com/json';
    $.ajax({
        type: 'GET',
        url: url,
        success: function (coordinates, type, xhr) {
            if (indicatior == 1) latLong = coordinates.lat;
            else if (indicatior == 2) latLong = coordinates.lon;
        },
        error: function (xhr) {
            window.alert('error: ' + xhr.statusText);
        }
    });
    return latLong;
}
