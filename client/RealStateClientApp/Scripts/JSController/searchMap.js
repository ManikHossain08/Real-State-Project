var rectangle;
var map;
var infoWindow;
var salesObject;
var seachMatchObj;
var datasForGO;
var NorthEast;
var SouthWest;
var marker, i;
var locations = [];
var markers = [];
output = [], output1 = [];

function initMap() {
    //var url = 'http://ip-api.com/json';
    //$.ajax({
    //    type: 'GET',
    //    url: url,
    //    success: function (coordinates, type, xhr) {

    //        var lattitude = coordinates.lat; //23.777706520371947;
    //        var longitude = coordinates.lon; //90.34332275390625;
    $.blockUI({ message: '<h5><img src="http://initvent.net/RealEstateSearch/Images/ajax-loader.gif" /> Loading Map, Please wait...</h5>' });
    //var height = screen.height;
    //var width = screen.width;
    //if (height <= 720) { document.getElementById('searchDataInfoTable').style.marginTop = "133%"; }
    //if (height >= 768) { document.getElementById('searchDataInfoTable').style.marginTop = "136%"; }
    //if (height > 770) { document.getElementById('searchDataInfoTable').style.marginTop = "142%"; }
    //if (height > 900) { document.getElementById('searchDataInfoTable').style.marginTop = "170%"; }
    //if (height >= 1000) { document.getElementById('searchDataInfoTable').style.marginTop = "180%"; }


    var lattitude = 55.715066613875599;
    var longitude = 12.464052296755700;
    populateDate();
    populateDate1();

    //var url = 'http://initvent.biz/GoogleAPISvc/AdministrationService.svc/SalesObjects';
    //$.ajax({
    //    type: 'GET',
    //    url: url,
    //    success: function (salesObjects, type, xhr) {
    $.unblockUI();
    salesObject = [];//salesObjects;
    map = new google.maps.Map(document.getElementById('map'), {
        //center: { lat: 44.5452, lng: -78.5389 },
        center: { lat: lattitude, lng: longitude },
        zoom: 13,
        mapTypeId: 'roadmap'
    });


    var input = document.getElementById('pac-input');
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    var autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.bindTo('bounds', map);



    autocomplete.addListener('place_changed', function () {
        var place = autocomplete.getPlace();
        setSpecificPlaceRectangle(place);
    });

    var bounds = {
        north: lattitude + .0100000,//44.599,
        south: lattitude - .00000001,//44.490,
        east: longitude + .0100000,//-78.443,
        west: longitude - .00000001//-78.649
    };

    rectangle = new google.maps.Rectangle({
        bounds: bounds,
        editable: true,
        draggable: true,
        fillOpacity: .001,
        //fillColor: '#ffff00',
        strokeWeight: .8
    });
    NorthEast = rectangle.getBounds().getNorthEast();
    SouthWest = rectangle.getBounds().getSouthWest();
    rectangle.setMap(map);
    rectangle.addListener('bounds_changed', showNewRect);
    infoWindow = new google.maps.InfoWindow();


    //    },
    //    error: function (xhr) {
    //        window.alert('error: ' + xhr.statusText);
    //    }
    //});

    //    },
    //    error: function (xhr) {
    //        window.alert('error: ' + xhr.statusText);
    //    }
    //});

}


function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
    markers = [];
}

function clearMarkers() {
    setMapOnAll(null);
}

function callControllerFunction() {

    var noOfBedroom = Number(document.getElementById('noOfBedroom').value);
    var noOfBedroomLo = noOfBedroom - ((noOfBedroom * 20) / 100);
    var noOfBedroomHI = noOfBedroom + ((noOfBedroom * 20) / 100);
    var Price1 = Number(document.getElementById('Price').value);
    var Price = Price1 + ((Price1 * 20) / 100);
    var Distance1 = Number(document.getElementById('Distance').value * 1000);
    var Distance = Distance1 + ((Distance1 * 20) / 100);
    var travelType = document.getElementById('travelType').value;
    var objectType = document.getElementById('objectType').value;
    if (noOfBedroom && Price1 && Distance1 && travelType && objectType) {
        $.blockUI({ message: '<h5><img src="http://initvent.net/RealEstateSearch/Images/ajax-loader.gif" /> Searching , Please wait...</h5>' });
        var url = 'http://initvent.net/GoogleAPISvc/AdministrationService.svc/searchSalesObjComparedMapObject?noOfBedroom=' + noOfBedroomHI + '&Price=' + Price + '&Distance=' + Distance + '&travelType=' + travelType + '&objectType=' + objectType;
        $.ajax({
            type: 'GET',
            url: url,
            success: function (salesObjects, type, xhr) {
                salesObject = salesObjects.searchSalesObject;
                seachMatchObj = salesObjects.searchSalesObject;
                searchFilterDataFunction();
                populateDate();
                populateDate1();
                $.unblockUI();
            },
            error: function (xhr) {
                $.unblockUI();
                //window.alert('error: ' + xhr.statusText);
            }
        });
    } else {
        infoWindow.setContent(contentString = '<p><span id="message">Please provide all input combination.</span></p>');
        infoWindow.setPosition(NorthEast);
        infoWindow.open(map);
        document.getElementById('message').style.color = 'RED';
    }

}

function populateDate() {
    //Build an array containing Customer records.
    var list1 = new Array();
    list1.push(["#", "Sales Object", "Zip",  "Rooms", "price", "Distance", "Time", "Size", "Pets Allowed"]);
    for (var i = 0; i < output1.length; i++) {
        list1.push([i + 1, output1[i].nameOfObject, output1[i].floor, output1[i].numberOfBedrooms, output1[i].price, output1[i].distanceInKM, output1[i].timeInMins, output1[i].size, output1[i].petsAllowed, output1[i].lift]); 
    }
    var table = document.createElement("TABLE");
    table.border = "1";
    table.className = "table table-bordered table-hover";
    var columnCount = list1[0].length;

    //Add the header row.
    var row = table.insertRow(-1);
    for (var i = 0; i < columnCount; i++) {
        var headerCell = document.createElement("TH");
        headerCell.innerHTML = list1[0][i];
        row.appendChild(headerCell);
    }

    //Add the data rows.
    for (var i = 1; i < list1.length; i++) {
        row = table.insertRow(-1);
        for (var j = 0; j < columnCount; j++) {
            var cell = row.insertCell(-1);
            if (j == 1) {
                cell.innerHTML = '<a href="' + list1[i][columnCount] + '" target="_blank" title="Clik to go to the link">' + list1[i][j] + '</a> ' //list1[i][j];
            }
            else  cell.innerHTML = list1[i][j];
        }
    }

    var dvTable = document.getElementById("dvTable");
    dvTable.innerHTML = "";
    dvTable.appendChild(table);

}

function populateDate1() {
    //Build an array containing Customer records.
    var list2 = new Array();
    list2.push(["#", "Sales Object", "Zip", "Rooms", "price", "Distance", "Time", "Size", "Pets Allowed"]);
    for (var i = 0; i < output.length; i++) {
        list2.push([i + 1, output[i].nameOfObject, output[i].floor, output[i].numberOfBedrooms, output[i].price, output[i].distanceInKM, output[i].timeInMins, output[i].size, output[i].petsAllowed, output[i].lift]);
    }

    //Create a HTML Table element.
    var table = document.createElement("TABLE");
    table.border = "1";
    table.className = "table table-bordered table-hover";
    //table.id = "searchDataInfoTable";

    //Get the count of columns.
    var columnCount = list2[0].length;

    //Add the header row.
    var row = table.insertRow(-1);
    for (var i = 0; i < columnCount; i++) {
        var headerCell = document.createElement("TH");
        headerCell.innerHTML = list2[0][i];
        row.appendChild(headerCell);
    }

    //Add the data rows.
    for (var i = 1; i < list2.length; i++) {
        row = table.insertRow(-1);
        for (var j = 0; j < columnCount; j++) {
            var cell = row.insertCell(-1);
            if (j == 1) {
                cell.innerHTML = '<a href="' + list2[i][columnCount] + '" target="_blank" title="Clik to go to the link">' + list2[i][j] + '</a> ' //list1[i][j];
            }
            else cell.innerHTML = list2[i][j];
        }
    }

    var dvTable = document.getElementById("dvTable1");
    dvTable.innerHTML = "";
    dvTable.appendChild(table);

}



function searchFilterDataFunction() {
    clearMarkers();
    var datasForGO = [];
    var datasForMissList = [];
    var MissList = [];
    var noOfBedroom = Number(document.getElementById('noOfBedroom').value);
    var noOfBedroomLo = noOfBedroom - ((noOfBedroom * 20) / 100);
    var noOfBedroomHI = noOfBedroom + ((noOfBedroom * 20) / 100);
    var Price1 = Number(document.getElementById('Price').value);
    var Price = Price1 + ((Price1 * 20) / 100);
    var Distance1 = Number(document.getElementById('Distance').value * 1000);
    var Distance = Distance1 + ((Distance1 * 20) / 100);
    var travelType = document.getElementById('travelType').value;
    var objectType = document.getElementById('objectType').value;
    var contentString = '';
    var msgcolor = '';
    var msgcolor1 = '';
    if (Distance) {
        if (seachMatchObj && seachMatchObj.length > 0) {
            for (var i = 0; i < seachMatchObj.length; i++) {
                point = new google.maps.LatLng(seachMatchObj[i].lattitude, seachMatchObj[i].longitude);
                isWithinRectangle = rectangle.getBounds().contains(point);
                if (isWithinRectangle && (noOfBedroomHI >= seachMatchObj[i].numberOfBedrooms && seachMatchObj[i].numberOfBedrooms >= noOfBedroomLo) && Price >= seachMatchObj[i].price && Distance >= seachMatchObj[i].distance && travelType == seachMatchObj[i].travelType && objectType == seachMatchObj[i].objType) {
                    datasForGO.push(seachMatchObj[i]);
                } else {
                    if (isWithinRectangle) datasForMissList.push(seachMatchObj[i]);
                }
            }
        }
        else {
            seachMatchObj = [];
            for (var i = 0; i < salesObject.length; i++) {
                point = new google.maps.LatLng(salesObject[i].lattitude, salesObject[i].longitude);
                isWithinRectangle = rectangle.getBounds().contains(point);
                if (isWithinRectangle && (noOfBedroomHI >= salesObject[i].numberOfBedrooms >= noOfBedroomLo) && Price >= salesObject[i].price && Distance >= salesObject[i].distance && travelType == salesObject[i].travelType && objectType == salesObject[i].objType) {
                    datasForGO.push(salesObject[i]);
                } else {
                    if (isWithinRectangle) datasForMissList.push(salesObject[i]);
                }
            }

        }

        //For near Miss-list Search
        var flag = 0;
        for (var i = 0; i < datasForMissList.length; i++) {
            flag = 0;
            if (noOfBedroom == datasForMissList[i].numberOfBedrooms && Price1 >= datasForMissList[i].price && travelType == datasForMissList[i].travelType && objectType == datasForMissList[i].objType) {
                if (flag == 0) {
                    MissList.push(datasForMissList[i]);
                    flag = 1;
                }
            }
            if (noOfBedroom == datasForMissList[i].numberOfBedrooms && Distance1 >= datasForMissList[i].distance && travelType == datasForMissList[i].travelType && objectType == datasForMissList[i].objType) {
                if (flag == 0) {
                    MissList.push(datasForMissList[i]);
                    flag = 1;
                }
            }
            if (Price1 >= datasForMissList[i].price && Distance1 >= datasForMissList[i].distance && travelType == datasForMissList[i].travelType && objectType == datasForMissList[i].objType) {
                if (flag == 0) {
                    MissList.push(datasForMissList[i]);
                    flag = 1;
                }
            }

        }
        var flags = [], l = MissList.length, i;
        output = [];
        for (i = 0; i < l; i++) {
            if (flags[MissList[i].nameOfObject]) continue;
            flags[MissList[i].nameOfObject] = true;
            output.push(MissList[i]);
        }

        var flags1 = [], l = datasForGO.length, i;
        output1 = [];
        for (i = 0; i < l; i++) {
            if (flags1[datasForGO[i].nameOfObject]) continue;
            flags1[datasForGO[i].nameOfObject] = true;
            output1.push(datasForGO[i]);
        }

        showMarkerOnMap(output1, output);
      //angular.element(document.getElementById('searchMapByRectangle')).scope().PopulateSearchData(output1, output);
        if (output1.length > 0) {
            contentString = '<p><span id="message">' + output1.length + ' Records found on this area </span></p>';
            if (output.length > 0) {
                contentString += '<p><span id="message1">And ' + output.length + ' Records found for Near-Miss List </span></p>';
                msgcolor1 = "GREEN";
            }
            msgcolor = "GREEN";
        } else {
            contentString = '<p><span id="message">No record found on this area & input combinition.</span></p>';
            if (output.length > 0) {
                contentString += '<p><span id="message1">But ' + output.length + ' Records found for Near-Miss List </span></p>';
                msgcolor1 = "GREEN";
            }
            msgcolor = "RED";
        }
    }
    else {
        contentString = '<p><span id="message">You have forgot to to indicate dinstance.</span></p>';
        msgcolor = "RED";
    }
    infoWindow.setContent(contentString);
    infoWindow.setPosition(NorthEast);
    infoWindow.open(map);
    document.getElementById('message').style.color = msgcolor;
    if (document.getElementById('message1')) document.getElementById('message1').style.color = msgcolor1;
}



function showMarkerOnMap(output1, output) {

    // set multiple marker
    for (var i = 0; i < output1.length; i++) {
        // init markers
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(output1[i].lattitude, output1[i].longitude),
            map: map,
            title: output1[i].nameOfObject
        });
        markers.push(marker);
        // process multiple info windows
        (function (marker, i) {
            // add click event
            google.maps.event.addListener(marker, 'click', function () {
                infoWindow = new google.maps.InfoWindow({
                    content: marker.title
                });
                infoWindow.open(map, marker);
            });
        })(marker, i);
    }

    for (var i = 0; i < output.length; i++) {
        // init markers
        marker = new google.maps.Marker({
            position: new google.maps.LatLng(output[i].lattitude, output[i].longitude),
            map: map,
            title: output[i].nameOfObject
        });
        markers.push(marker);
        // process multiple info windows
        (function (marker, i) {
            // add click event
            google.maps.event.addListener(marker, 'click', function () {
                infoWindow = new google.maps.InfoWindow({
                    content: marker.title
                });
                infoWindow.open(map, marker);
            });
        })(marker, i);
    }

}

function showNewRect(event) {
    var point;
    var isWithinRectangle;
    //seachMatchObj = [];
    NorthEast = rectangle.getBounds().getNorthEast();
    SouthWest = rectangle.getBounds().getSouthWest();
    //for (var i = 0; i < salesObject.length; i++) {
    //    point = new google.maps.LatLng(salesObject[i].lattitude, salesObject[i].longitude);
    //    isWithinRectangle = rectangle.getBounds().contains(point);
    //    if (isWithinRectangle) {
    //        seachMatchObj.push(salesObject[i]);
    //    }
    //}
}

function setSpecificPlaceRectangle(place) {

    lattitude = place.geometry.location.lat();
    longitude = place.geometry.location.lng();

    map = new google.maps.Map(document.getElementById('map'), {
        //center: { lat: 44.5452, lng: -78.5389 },
        center: { lat: lattitude, lng: longitude },
        zoom: 13,
        mapTypeId: 'roadmap'
    });

    var bounds = {
        north: lattitude + .0100000,//44.599,
        south: lattitude - .00000001,//44.490,
        east: longitude + .0100000,//-78.443,
        west: longitude - .00000001//-78.649
    };

    rectangle = new google.maps.Rectangle({
        bounds: bounds,
        editable: true,
        draggable: true,
        fillOpacity: .001,
        strokeWeight: .8
    });
    NorthEast = rectangle.getBounds().getNorthEast();
    SouthWest = rectangle.getBounds().getSouthWest();
    rectangle.setMap(map);
    rectangle.addListener('bounds_changed', showNewRect);
    infoWindow = new google.maps.InfoWindow();

    $('.appendSearchBox').remove();

    var div = document.createElement('div');
    div.className = 'appendSearchBox';
    div.innerHTML = '<input id="pac-input" class="controls" type="text" placeholder="Search location to placed rectangle ....">';
    document.getElementById('appendBox').appendChild(div);

    var input = document.getElementById('pac-input');
    map.controls[google.maps.ControlPosition.TOP_LEFT].push(input);

    var autocomplete = new google.maps.places.Autocomplete(input);
    autocomplete.bindTo('bounds', map);
    autocomplete.addListener('place_changed', function () {
        var place = autocomplete.getPlace();
        setSpecificPlaceRectangle(place);
    });
}

