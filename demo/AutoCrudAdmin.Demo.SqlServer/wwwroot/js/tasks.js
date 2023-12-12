(function(){
    [...document.getElementsByTagName("td")]
        .forEach((td,index)=> td.style.color = index % 2 === 0 ? "red" : "green")
})()