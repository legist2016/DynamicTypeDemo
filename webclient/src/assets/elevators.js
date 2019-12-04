/*
{
    init: function(elevators, floors) {
        function between(num, left, right) {
            return Math.abs(left - right) == Math.abs(left - num) + Math.abs(num - right)
        }

        function delDest(m, e, f) {

            if (e.constructor == Array) {
                for (i in e) {
                    delDest(m, e[i], f)
                }
            } else {
                if (m == e) return
                ed = e.destinationQueue
                console.log('elevator', e.id, "delete", f, "queue:", ed, "pressed:", e.getPressedFloors())
                if (!e.getPressedFloors().includes(f)) {
                    index = ed.findIndex(a => a == f)
                    if (index >= 0) {
                        ed.splice(i, 1)
                        console.log('elevator', e.id, "deleted", f, "queue:", ed)
                    }
                }
            }
        }

        function addDest(e, f, d) {

            if (e.constructor == Array && d != "out") {
                for (i in e) {
                    addDest(e[i], f, d)
                }
            } else {
                ed = e.destinationQueue
                if(e.destinationDirection=="stop" && e.currentFloor()==f && e.loadFactor<1){
                    return
                }
                if (ed.length == 0) {
                    console.log("elevator", e.id, "at", e.currentFloor(), "goto", f)
                    e.goToFloor(f)
                } else {
                    if (ed.find(a => a == f) == undefined) {
                        for (i in ed) {
                            left = i == 0 ? e.currentFloor() : ed[i - 1]
                            right = ed[i]
                            if (between(f, left, right)) {
                                console.log("elevator", e.id, "at", e.currentFloor(), "insert", f, "to", ed)
                                ed.splice(i, 0, f)
                                f = -1
                                break;
                            }
                        }
                        if (f >= 0) {
                            for (i in ed) {
                                if (i > 0 && ed[i + 1] != undefined) {
                                    if (
                                        (
                                            ed[i] > ed[i - 1] &&
                                            ed[i] > ed[i + 1] &&
                                            f > ed[i]
                                        ) ||
                                        (
                                            ed[i] < ed[i - 1] &&
                                            ed[i] < ed[i + 1] &&
                                            f < ed[i]
                                        )) {
                                        console.log("elevator", e.id, "at", e.currentFloor(), "insert", f, "to", ed)
                                        ed.splice(i, 0, f)
                                        f = -1
                                        break;
                                    }
                                }
                            }
                        }
                        if (f >= 0) {
                            console.log("elevator", e.id, "at", e.currentFloor(), "append", f, "to", ed)
                            ed.push(f)
                        }
                        console.log("elevator", e.id, "queue:", ed)
                        e.checkDestinationQueue()
                    }
                }
            }
        }
        elevators.forEach(elevator => {
            //var elevator = elevators[i]; // Let's use the first elevator
            elevator.id = elevators.findIndex(a => a == elevator)
            elevator.on("idle", function () { });

            elevator.on("floor_button_pressed", function (floorNum) {
                console.log("elevator", this.id, "press", floorNum)
                addDest(this, floorNum, "out")
            });
            elevator.on("stopped_at_floor", function (floorNum) {
                console.log("elevator", this.id, "stop at", floorNum)
                delDest(this, elevators, floorNum)
            });
        });
        floors.forEach(floor => {
            floor.on("up_button_pressed", function () {
                console.log("floor", this.floorNum(), "press UP")
                addDest(elevators, this.floorNum(), "up")
            });
            floor.on("down_button_pressed", function () {
                console.log("floor", this.floorNum(), "press DOWN")
                addDest(elevators, this.floorNum(), "down")
            });
        });

    },
    update: function(dt, elevators, floors) {

    }
}
*/