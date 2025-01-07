//function applyBackgroundTint() {
//    const hour = new Date().getHours();
//    const body = document.body;

//    console.log('Current hour:', hour); // Log the current hour

//    if ((hour >= 6 && hour < 9) || (hour >= 17 && hour < 19)) {
//        console.log('Applying morning-dusk tint');
//        body.classList.add('morning-dusk');
//    } else if (hour >= 9 && hour < 17) {
//        console.log('Applying daytime tint');
//        body.classList.add('daytime');
//    } else {
//        console.log('Applying nighttime tint');
//        body.classList.add('nighttime');
//    }
//}

//// Apply tint only once per session
//if (!sessionStorage.getItem('tintApplied')) {
//    console.log('Applying tint for the first time this session');
//    applyBackgroundTint();
//    sessionStorage.setItem('tintApplied', 'true');
//} else {
//    console.log('Tint already applied this session');
//}