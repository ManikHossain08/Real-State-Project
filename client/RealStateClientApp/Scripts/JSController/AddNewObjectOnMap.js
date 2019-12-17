var markers = [];
var map;
var lattitude = 0;
var longitude = 0;
var directionsService;
var distanceAndTimeObj;
var distanceAndTime = [];
var countInsert = 0;
var objType = '';
//var marker;
function initAutocomplete() {
    //var url = 'http://ip-api.com/json';
    //$.ajax({
    //    type: 'GET',
    //    url: url,
    //    success: function (coordinates, type, xhr) {
    //        lattitude = coordinates.lat;
    //        longitude = coordinates.lon;

            lattitude = 55.715066613875599;
            longitude = 12.464052296755700;
            map = new google.maps.Map(document.getElementById('map'), {
                center: { lat: lattitude, lng: longitude },
                zoom: 13,
                mapTypeId: 'roadmap'
            });

            directionsService = new google.maps.DirectionsService;

            // Create the search box and link it to the UI element.
            var input = document.getElementById('pac-input');
            var searchBox = new google.maps.places.SearchBox(input);
            map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

            // Bias the SearchBox results towards current map's viewport.
            map.addListener('bounds_changed', function () {
                searchBox.setBounds(map.getBounds());
            });

            google.maps.event.addListener(map, 'click', function (event) {
                setMarkerValue(event.latLng.lat(), event.latLng.lng(), event,true);
            });


            function setMarkerValue(lat, long, event,isclick) {
                document.getElementById('Lattitude').value = lat;
                document.getElementById('Longitude').value = long;
                document.getElementById('Address').value = '';
                if (isclick) addMarker(event.latLng);
            }

            function populatePlaceBySearch(lat, long, placeName) {
                document.getElementById('Lattitude').value = lat;
                document.getElementById('Longitude').value = long;
                document.getElementById('Address').value = document.getElementById('pac-input').value;
            }

            function addMarker(location) {
              var  marker = new google.maps.Marker({
                    position: location,
                    map: map,
                    draggable: true
                });
              markers.push(marker);
              google.maps.event.addListener(marker, 'dragend', function (event) {
                  setMarkerValue(event.latLng.lat(), event.latLng.lng(), event,false);
              });

              google.maps.event.addListener(marker, 'click', function () {
                  infowindow.setContent('<div><strong>' + place.name + '</strong><br>' +
                    'Place ID: ' + place.place_id + '<br>' +
                    place.formatted_address + '</div>');
                  infowindow.open(map, this);
              });

            }


            function handleEvent(event) {
                document.getElementById('lat').value = event.latLng.lat();
                document.getElementById('lng').value = event.latLng.lng();
            }

            // Listen for the event fired when the user selects a prediction and retrieve
            // more details for that place.
            searchBox.addListener('places_changed', function () {
                var places = searchBox.getPlaces();

                if (places.length == 0) {
                    return;
                }

                // Clear out the old markers.
                markers.forEach(function (marker) {
                    marker.setMap(null);
                });
                markers = [];

                // For each place, get the icon, name and location.
                var bounds = new google.maps.LatLngBounds();
                places.forEach(function (place) {
                    if (!place.geometry) {
                        console.log("Returned place contains no geometry");
                        return;
                    }
                    var icon = {
                        url: place.icon,
                        size: new google.maps.Size(71, 71),
                        origin: new google.maps.Point(0, 0),
                        anchor: new google.maps.Point(17, 34),
                        scaledSize: new google.maps.Size(25, 25)
                    };
                    var mainmarker = new google.maps.Marker({
                        map: map,
                        icon: icon,
                        title: place.name,
                        position: place.geometry.location,
                        draggable: true
                    })
                    // Create a marker for each place.
                    markers.push(mainmarker);
                    populatePlaceBySearch(place.geometry.location.lat(), place.geometry.location.lng(), place.name);
                    google.maps.event.addListener(mainmarker, 'dragend', function (event) {
                        setMarkerValue(event.latLng.lat(), event.latLng.lng(), event,false);
                    });

                    if (place.geometry.viewport) {
                        // Only geocodes have viewport.
                        bounds.union(place.geometry.viewport);
                    } else {
                        bounds.extend(place.geometry.location);
                    }
                });
                map.fitBounds(bounds);
            });
    //    },
    //    error: function (xhr) {
    //        window.alert('error: ' + xhr.statusText);
    //    }
    //});
}


function SaveMarkerCoOrdinate() {
    var errorMsg = "";
    var lat = document.getElementById('Lattitude').value;
    var long = document.getElementById('Longitude').value;

    var objName = document.getElementById('ObjName').value;
    var Address = document.getElementById('Address').value;
     objType = document.getElementById('objectType').value;

    if (lat == "") { errorMsg = "Please select any object on map by mouse click."; }
    if (objName == "") { errorMsg = "Please give an object name to continue."; }
    if (Address == "") { errorMsg = "Please give an Address to continue."; }
    if (objType == "") { errorMsg = "Please select an object Type to continue."; }

    if (errorMsg == "") {
        document.getElementById('message').innerHTML = "";
        document.getElementById('message').style.color = "BLACK";
        if (confirm('Are you want to save this map object?') == true) {
            $.blockUI({ message: '<h5><img src="http://initvent.net/RealEstateSearch/Images/ajax-loader.gif" /> Populating Data, Please wait...</h5>' });
            var url = 'http://maps.googleapis.com/maps/api/geocode/json?latlng=' + lat + ',' + long + '&sensor=true'
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                success: function (response, type, xhr) {
                    placeId = response.results[1].place_id;
                    var url = 'http://10.1.15.27/GoogleSvc/AdministrationService.svc/createMapObject?objectName=' + objName + '&lat=' + lat + '&longitude=' + long + '&address=' + Address + '&placeId=' + placeId + '&objType=' + objType;
                    $.ajax({
                        type: 'GET',
                        url: url,
                        dataType: 'json',
                        success: function (response, type, xhr) {
                            distanceAndTime = [];
                            //alert('Please wait a while untill succesfull message.It takes few minute.');
                            //for (var i = 0; i < response.SalesObjectList.length; i++) {
                            //    if (response.SalesObjectList.length > 0) getDistanceAndDurationByDriving(response.SalesObjectList[i].placeId, placeId, 'DRIVING');
                            //    if (response.SalesObjectList.length > 0) getDistanceAndDurationByDriving(response.SalesObjectList[i].placeId, placeId, 'WALKING');
                            //}
                            //if (distanceAndTime.length > 0) {
                            //    countInsert = 0;
                            //    insertValue(distanceAndTime[0].Source_id, distanceAndTime[0].Destinition_id, distanceAndTime[0].TravelType, distanceAndTime[0].Distance, distanceAndTime[0].Time, distanceAndTime[0].DistanceInKM);
                            //}
                            //else ClearObjectValue();

                           
                            if (response.SalesObjectList.length > 0) getDistanceAndDurationByDriving(response.SalesObjectList[0].placeId, placeId, 'DRIVING');
                            if (response.SalesObjectList.length > 0) getDistanceAndDurationByWalking(response.SalesObjectList[0].placeId, placeId, 'WALKING');
                            else ClearObjectValue();
                           
                        },
                        error: function (xhr) {
                            window.alert('error: ' + xhr.statusText);
                            $.unblockUI();
                        }
                    });
                },
                error: function (xhr) {
                    window.alert('error: ' + xhr.statusText);
                    $.unblockUI();
                }
            });
        }
    } else {
        document.getElementById('message').innerHTML = '***'+errorMsg;
        document.getElementById('message').style.color = "RED";
    }
}



function ClearObjectValue() {
    document.getElementById('Lattitude').value = '';
    document.getElementById('Longitude').value = '';
    document.getElementById('ObjName').value = '';
    document.getElementById('Address').value = '';
    document.getElementById('objectType').value = '';
    document.getElementById('message').innerHTML = "Successfully saved map object information.";
    document.getElementById('message').style.color = "GREEN";
    $.unblockUI();
}


function insertValue(Source_id, Destinition_id, TravelType, Distance, Time, DistanceInKM) {
    var url = 'http://10.1.15.27/GoogleSvc/AdministrationService.svc/createComparedMapObject?Source_id=' + Source_id + '&Destinition_id=' + Destinition_id + '&TravelType=' + TravelType + '&Distance=' + Distance + '&Time=' + Time + '&DistanceInKM=' + DistanceInKM + '&objType=' + objType;
    $.ajax({
        type: 'GET',
        url: url,
        dataType: 'json',
        success: function (response, type, xhr) {
            countInsert += 1
            if (distanceAndTime.length != countInsert && distanceAndTime.length > countInsert)
                insertValue(distanceAndTime[countInsert].Source_id, distanceAndTime[countInsert].Destinition_id, distanceAndTime[countInsert].TravelType, distanceAndTime[countInsert].Distance, distanceAndTime[countInsert].Time, distanceAndTime[countInsert].DistanceInKM);
            else ClearObjectValue();
        },
        error: function (xhr) {
            window.alert('error: ' + xhr.statusText);
        }
    });
}




function getDistanceAndDurationByDriving(destinationId, sourceObjplaceId, travelType) {
    var request = {
        origin: {
            placeId: sourceObjplaceId
        },
        destination: {
            placeId: destinationId
        },
        travelMode: travelType //DRIVING
    };
    directionsService.route(request, function (response, status) {
        //alert(status);
        if (status == google.maps.DirectionsStatus.OK) {
            var url = 'http://10.1.15.27/GoogleSvc/AdministrationService.svc/createComparedMapObject?Source_id=' + response.request.origin.placeId + '&Destinition_id=' + response.request.destination.placeId + '&TravelType=' + response.request.travelMode + '&Distance=' + response.routes[0].legs[0].distance.value + '&Time=' + response.routes[0].legs[0].duration.value + '&DistanceInKM=' + response.routes[0].legs[0].distance.text + '&objType=' + objType;
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                async:false,
                success: function (response, type, xhr) {
                    //ClearObjectValue(response.savedObject, lat, long, placeId);
                    //document.getElementById('message').innerHTML = response;
                    if (response.SalesObjectList.length > 0)
                        setTimeout(function () { getDistanceAndDurationByDriving(response.SalesObjectList[0].placeId, response.placeId, response.travelType) }, 1500);
                    else ClearObjectValue();
                },
                error: function (xhr) {
                    window.alert('error: ' + xhr.statusText);
                }
            });
        }
    });
}





function getDistanceAndDurationByWalking(destinationId, sourceObjplaceId, travelType) {
    var request = {
        origin: {
            placeId: sourceObjplaceId
        },
        destination: {
            placeId: destinationId
        },
        travelMode: travelType //DRIVING
    };
    directionsService.route(request, function (response, status) {
        if (status == google.maps.DirectionsStatus.OK) {
            var url = 'http://10.1.15.27/GoogleSvc/AdministrationService.svc/createComparedMapObject?Source_id=' + response.request.origin.placeId + '&Destinition_id=' + response.request.destination.placeId + '&TravelType=' + response.request.travelMode + '&Distance=' + response.routes[0].legs[0].distance.value + '&Time=' + response.routes[0].legs[0].duration.value + '&DistanceInKM=' + response.routes[0].legs[0].distance.text + '&objType=' + objType;
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                success: function (response, type, xhr) {
                    //ClearObjectValue(response.savedObject, lat, long, placeId);
                    //document.getElementById('message').innerHTML = response;
                    if (response.SalesObjectList.length > 0)
                        setTimeout(function () {getDistanceAndDurationByWalking(response.SalesObjectList[0].placeId, response.placeId, response.travelType)},1500);
                    else ClearObjectValue();
                },
                error: function (xhr) {
                    window.alert('error: ' + xhr.statusText);
                }
            });
        }
    });
}



function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
}

function clearMarkers() {
    setMapOnAll(null);
}

function showMarkers() {
    setMapOnAll(map);
}

function deleteMarkers() {
    clearMarkers();
    markers = [];
}


function rad(x) {
    return x * Math.PI / 180;
}


function getDistance(p1, p2) {
    var R = 6378137; // Earth’s mean radius in meter
    var dLat = rad(p2.lat() - p1.lat());
    var dLong = rad(p2.lng() - p1.lng());
    var a = Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(rad(p1.lat())) * Math.cos(rad(p2.lat())) *
      Math.sin(dLong / 2) * Math.sin(dLong / 2);
    var c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    var d = R * c;
    return d; // returns the distance in meter
}







///////////////////Not Used Funtion are bellow just kep these for demo or example function//////////////////

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



    //$.ajax({
    //    type: 'POST',
    //    url: 'Services/RestTestService.svc/PostTest',
    //    dataType: 'json',
    //    contentType: 'application/json',
    //    data: '{ "a": "a", "b": "b" }',
    //    success: function (response, type, xhr) {
    //        window.alert('A: ' + response.PostTestResult.A);
    //    },
    //    error: function (xhr) {
    //        window.alert('error: ' + xhr.statusText);
    //    }
    //});
    //var value = { Title: lat, Lat: lat, Long: long, note: Note, placeId: placeId };
    // var url = 'http://10.1.15.27/GoogleSvc/AdministrationService.svc/testMapObject';
    //$.ajax({
    //    type: 'GET',
    //    url: url,
    //    dataType: 'json',
    //    contentType: 'application/json; charset=utf-8',
    //    data: JSON.stringify(value),
    //    success: function (response, type, xhr) {
    //        ClearObjectValue();
    //    },
    //    error: function (xhr) {
    //        window.alert('error: ' + xhr.statusText);
    //    }
    //});

    return latLong;
}

