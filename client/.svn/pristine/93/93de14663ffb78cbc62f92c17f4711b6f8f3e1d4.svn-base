var markers = [];
var map;
var lattitude = 0;
var longitude = 0;
var directionsService;
var distanceAndTimeObj;
var distanceAndTime = [];
var countInsert = 0;
var rawMapObj = [];
var geocoder;
var objLength = 0;
var address = '';
var type = '';
var id = '';
//var marker;
function initAutocomplete() {
    //var url = 'http://ip-api.com/json';
    //$.ajax({
    //    type: 'GET',
    //    url: url,
    //    success: function (coordinates, type, xhr) {
    //        lattitude = coordinates.lat;
    //        longitude = coordinates.lon;


    geocoder = new google.maps.Geocoder();
    var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/getRawMapObject';
    $.ajax({
        type: 'GET',
        url: url,
        success: function (RawMapObject, type, xhr) {
            rawMapObj = RawMapObject;
            var MapObject = document.getElementById('MapObject');
            for (var i = 0; i < RawMapObject.length; i++) {
                var optstart = document.createElement('option');
                optstart.innerHTML = RawMapObject[i].address;
                optstart.value = RawMapObject[i].LocationID;
                MapObject.appendChild(optstart);
            }
        },
        error: function (xhr) {
            window.alert('error: ' + xhr.statusText);
        }
    });



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
        setMarkerValue(event.latLng.lat(), event.latLng.lng(), event, true);
    });


    function setMarkerValue(lat, long, event, isclick) {
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

    function handleEvent(event) {
        document.getElementById('lat').value = event.latLng.lat();
        document.getElementById('lng').value = event.latLng.lng();
    }
}

function codeAddress() {

    var objType = document.getElementById('MapObject').value;
    for (var i = 0; i < rawMapObj.length; i++) {
        if (objType == rawMapObj[i].LocationID) {
            address = rawMapObj[i].address;
            type = rawMapObj[i].objType;
            id = rawMapObj[i].LocationID;
            geocoder.geocode({ 'address': address }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    processing();
                    $('#MapObject option[value="' + id + '"]').remove();
                    SaveCoOrdinate(results[0].geometry.location.lat(), results[0].geometry.location.lng(), address, address, type, id);
                } else {
                    $.unblockUI();
                    alert("Geocode was not successful for the following reason: " + status);  
                }
            });
        }
    }
}


function SaveCoOrdinate(lat, long, objName, Address, objType,id) {
    var url = 'http://maps.googleapis.com/maps/api/geocode/json?latlng=' + lat + ',' + long + '&sensor=true'
    $.ajax({
        type: 'GET',
        url: url,
        dataType: 'json',
        success: function (response, type, xhr) {
            placeId = response.results[1].place_id;
            var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/UpdateRawMapObject?objectName=' + objName + '&lat=' + lat + '&longitude=' + long + '&address=' + Address + '&placeId=' + placeId + '&objType=' + objType + '&id=' + id;
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                success: function (response, type, xhr) {
                    //setTimeout(function () { codeAddress() }, 1500);
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

function ClearObjectValue() {
    document.getElementById('message').innerHTML = "Successfully saved map object information.";
    document.getElementById('message').style.color = "GREEN";
    //$('div.test').unblock();
    alert("successfully Completed");
    $.unblockUI();
    location.reload(true);
}


function processing() {

    //$('body').block({
    //    message: '<h1>Processing</h1>',
    //    css: { border: '3px solid #a00' }
    //});
    $.blockUI({ message: '<h5><img src="http://initvent.net/RealEstateSearch/Images/ajax-loader.gif" /> Populating Data, Please wait...</h5>' });
    document.getElementById('message').innerHTML = "Data Processing please wait...";
    document.getElementById('message').style.color = "RED";
   
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
            var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/createComparedMapObject?Source_id=' + response.request.origin.placeId + '&Destinition_id=' + response.request.destination.placeId + '&TravelType=' + response.request.travelMode + '&Distance=' + response.routes[0].legs[0].distance.value + '&Time=' + response.routes[0].legs[0].duration.value + '&DistanceInKM=' + response.routes[0].legs[0].distance.text;
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                async: false,
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
            var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/createComparedMapObject?Source_id=' + response.request.origin.placeId + '&Destinition_id=' + response.request.destination.placeId + '&TravelType=' + response.request.travelMode + '&Distance=' + response.routes[0].legs[0].distance.value + '&Time=' + response.routes[0].legs[0].duration.value + '&DistanceInKM=' + response.routes[0].legs[0].distance.text;
            $.ajax({
                type: 'GET',
                url: url,
                dataType: 'json',
                success: function (response, type, xhr) {
                    //ClearObjectValue(response.savedObject, lat, long, placeId);
                    //document.getElementById('message').innerHTML = response;
                    if (response.SalesObjectList.length > 0)
                        setTimeout(function () { getDistanceAndDurationByWalking(response.SalesObjectList[0].placeId, response.placeId, response.travelType) }, 1500);
                    else ClearObjectValue();
                },
                error: function (xhr) {
                    window.alert('error: ' + xhr.statusText);
                }
            });
        }
    });
}

