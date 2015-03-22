function openSection(sectionID) {
    var id = "#menuClick" + sectionID;
    setTimeout(function() {
        $(function(){$(id).click();});
    }, 100);
}