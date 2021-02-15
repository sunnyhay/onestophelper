/*
 * The CosmosDB User Defined Function gets input from the user and calculates the 4 level possibilities for 
 * each college.
 * entry: MatchData row; userInput: user input with a structure like below
 * { Gpa, Sat: {Avg, Read, Math, Wrt}, Act: {Cum, Eng, Math, Wrt}, Rank}
 * 
 * output: 4 highly recommend; 2 recommend; 1 neutral; 0 not recommend; -1 no result
 * With all four metrics, choose sat if for this college sat related metrics are more than act;
 * otherwise use act. 
 * With three metrics (gpa, sat or act and rank), if at least two highly recommend (no neutral), 
 * it’s highly recommend; if two recommend (no neutral, i.e. one highly recommend) or 
 * two highly recommend with one neutral, it’s recommend; if only one recommend (one highly 
 * recommend one neutral) or two neutral (the other highly/recommend), it’s neutral; otherwise 
 * (including three neutral) it’s not recommend.
 * For only two available, both two highly recommend finally highly recommend; one highly and one 
 * recommend or two recommend is recommend; only one highly/recommend and one neutral is neutral; 
 * otherwise (including two neutral) not recommend.
 * For only one available, use that original.
 * No metric? No evaluation. 
 
 */

function match(entry, userInput) {
    const userGpa = userInput.Gpa;
    const userSat = userInput.Sat;
    const userAct = userInput.Act;
    const userRank = userInput.Rank;

    const collegeGpa = entry.Gpa;
    const collegeSat = entry.Sat;
    const collegeAct = entry.Act;

    let finalResult = -1;
    let gpaResult = -1;
    let satResult = -1;
    let actResult = -1;
    let rankResult = -1;

    if (userGpa != null && collegeGpa != null) {
        const collegeGpaRange = collegeGpa.vals;
        if (userGpa >= collegeGpaRange[0]) gpaResult = 4;
        else if (collegeGpaRange[1] != null && userGpa >= collegeGpaRange[1])
            gpaResult = 2;
        else if (collegeGpaRange[2] != null && userGpa >= collegeGpaRange[2])
            gpaResult = 1;
        else gpaResult = 0;
    }

    if (userSat != null && collegeSat != null) {
        const collegeSatReadRange = collegeSat.read;
        const collegeSatMathRange = collegeSat.math;
        const collegeSatWrtRange = collegeSat.wrt;
        const userSatRead = userSat.Read;
        const userSatMath = userSat.Math;
        const userSatWrt = userSat.Wrt;

        let satReadResult = -1;
        let satMathResult = -1;
        let satWrtResult = -1;

        if (collegeSatReadRange != null && userSatRead != null) {
            if (userSatRead >= collegeSatReadRange[0]) satReadResult = 4;
            else if (userSatRead >= collegeSatReadRange[1]) satReadResult = 2;
            else satReadResult = 1;
        }

        if (collegeSatMathRange != null && userSatMath != null) {
            if (userSatMath >= collegeSatMathRange[0]) satMathResult = 4;
            else if (userSatMath >= collegeSatMathRange[1]) satMathResult = 2;
            else satMathResult = 1;
        }

        if (collegeSatWrtRange != null && userSatWrt != null) {
            if (userSatWrt >= collegeSatWrtRange[0]) satWrtResult = 4;
            else if (userSatWrt >= collegeSatWrtRange[1]) satWrtResult = 2;
            else satWrtResult = 1;
        }

        // aggregate all together
        if (satReadResult > 0 && satMathResult > 0 && satWrtResult > 0) {
            // all three results available
            let tempResult = satReadResult + satMathResult + satWrtResult;
            if (tempResult >= 10) satResult = 4;
            else if (tempResult >= 6) satResult = 2;
            else if (tempResult == 5) satResult = 1;
            else satResult = 0;
        } else if (satReadResult > 0 && satMathResult > 0) {
            // no writing evaluation
            let tempResult = satReadResult + satMathResult;
            if (tempResult >= 6) satResult = 4;
            else if (tempResult >= 4) satResult = 2;
            else if (tempResult == 3) satResult = 1;
            else satResult = 0;
        } else if (satReadResult > 0 && satWrtResult > 0) {
            // no math evaluation
            let tempResult = satReadResult + satWrtResult;
            if (tempResult >= 6) satResult = 4;
            else if (tempResult >= 4) satResult = 2;
            else if (tempResult == 3) satResult = 1;
            else satResult = 0;
        } else if (satMathResult > 0 && satWrtResult > 0) {
            // no reading evaluation
            let tempResult = satMathResult + satWrtResult;
            if (tempResult >= 6) satResult = 4;
            else if (tempResult >= 4) satResult = 2;
            else if (tempResult == 3) satResult = 1;
            else satResult = 0;
        } else if (satReadResult > 0) {
            // only reading evaluation
            satResult = satReadResult;
        } else if (satMathResult > 0) {
            // only math evaluation
            satResult = satMathResult;
        } else if (satWrtResult > 0) {
            // only writing evaluation
            satResult = satWrtResult;
        } else {
            // either user does not provide any SAT input, or the college has no SAT scores. keep -1
        }
    }

    if (userAct != null && collegeAct != null) {
        const collegeActCumRange = collegeAct.cum;
        const collegeActEngRange = collegeAct.eng;
        const collegeActMathRange = collegeAct.math;
        const collegeActWrtRange = collegeAct.wrt;
        const userActCum = userAct.Cum;
        const userActEng = userAct.Eng;
        const userActMath = userAct.Math;
        const userActWrt = userAct.Wrt;

        let actCumResult = -1;
        let actEngResult = -1;
        let actMathResult = -1;
        let actWrtResult = -1;

        if (collegeActCumRange != null && userActCum != null) {
            if (userActCum >= collegeActCumRange[0]) actCumResult = 4;
            else if (userActCum >= collegeActCumRange[1]) actCumResult = 2;
            else actCumResult = 1;
        }

        if (collegeActEngRange != null && userActEng != null) {
            if (userActEng >= collegeActEngRange[0]) actEngResult = 4;
            else if (userActEng >= collegeActEngRange[1]) actEngResult = 2;
            else actEngResult = 1;
        }

        if (collegeActMathRange != null && userActMath != null) {
            if (userActMath >= collegeActMathRange[0]) actMathResult = 4;
            else if (userActMath >= collegeActMathRange[1]) actMathResult = 2;
            else actMathResult = 1;
        }

        if (collegeActWrtRange != null && userActWrt != null) {
            if (userActWrt >= collegeActWrtRange[0]) actWrtResult = 4;
            else if (userActWrt >= collegeActWrtRange[1]) actWrtResult = 2;
            else actWrtResult = 1;
        }

        // aggregate all together
        if (
            actCumResult > 0 &&
            actEngResult > 0 &&
            actMathResult > 0 &&
            actWrtResult > 0
        ) {
            let tempResult =
                actCumResult + actEngResult + actMathResult + actWrtResult;
            if (tempResult >= 14) actResult = 4;
            else if (tempResult >= 8) actResult = 2;
            else if (tempResult >= 6) actResult = 1;
            else actResult = 0;
        } else if (actCumResult > 0 && actEngResult > 0 && actMathResult > 0) {
            let tempResult = actCumResult + actEngResult + actMathResult;
            if (tempResult >= 10) actResult = 4;
            else if (tempResult >= 6) actResult = 2;
            else if (tempResult == 5) actResult = 1;
            else actResult = 0;
        } else if (actCumResult > 0 && actEngResult > 0 && actWrtResult > 0) {
            let tempResult = actCumResult + actEngResult + actWrtResult;
            if (tempResult >= 10) actResult = 4;
            else if (tempResult >= 6) actResult = 2;
            else if (tempResult == 5) actResult = 1;
            else actResult = 0;
        } else if (actCumResult > 0 && actMathResult > 0 && actWrtResult > 0) {
            let tempResult = actCumResult + actMathResult + actWrtResult;
            if (tempResult >= 10) actResult = 4;
            else if (tempResult >= 6) actResult = 2;
            else if (tempResult == 5) actResult = 1;
            else actResult = 0;
        } else if (actEngResult > 0 && actMathResult > 0 && actWrtResult > 0) {
            let tempResult = actEngResult + actMathResult + actWrtResult;
            if (tempResult >= 10) actResult = 4;
            else if (tempResult >= 6) actResult = 2;
            else if (tempResult == 5) actResult = 1;
            else actResult = 0;
        } else if (actCumResult > 0 && actEngResult > 0) {
            let tempResult = actCumResult + actEngResult;
            if (tempResult >= 6) actResult = 4;
            else if (tempResult >= 4) actResult = 2;
            else if (tempResult == 3) actResult = 1;
            else actResult = 0;
        } else if (actCumResult > 0 && actMathResult > 0) {
            let tempResult = actCumResult + actMathResult;
            if (tempResult >= 6) actResult = 4;
            else if (tempResult >= 4) actResult = 2;
            else if (tempResult == 3) actResult = 1;
            else actResult = 0;
        } else if (actCumResult > 0 && actWrtResult > 0) {
            let tempResult = actCumResult + actWrtResult;
            if (tempResult >= 6) actResult = 4;
            else if (tempResult >= 4) actResult = 2;
            else if (tempResult == 3) actResult = 1;
            else actResult = 0;
        } else if (actEngResult > 0 && actMathResult > 0) {
            let tempResult = actEngResult + actMathResult;
            if (tempResult >= 6) actResult = 4;
            else if (tempResult >= 4) actResult = 2;
            else if (tempResult == 3) actResult = 1;
            else actResult = 0;
        } else if (actEngResult > 0 && actWrtResult > 0) {
            let tempResult = actEngResult + actWrtResult;
            if (tempResult >= 6) actResult = 4;
            else if (tempResult >= 4) actResult = 2;
            else if (tempResult == 3) actResult = 1;
            else actResult = 0;
        } else if (actMathResult > 0 && actWrtResult > 0) {
            let tempResult = actMathResult + actWrtResult;
            if (tempResult >= 6) actResult = 4;
            else if (tempResult >= 4) actResult = 2;
            else if (tempResult == 3) actResult = 1;
            else actResult = 0;
        } else if (actCumResult > 0) {
            // only cumulative evaluation
            actResult = actCumResult;
        } else if (actEngResult > 0) {
            // only english evaluation
            actResult = actEngResult;
        } else if (actMathResult > 0) {
            // only math evaluation
            actResult = actMathResult;
        } else if (actWrtResult > 0) {
            // only writing evaluation
            actResult = actWrtResult;
        } else {
            // either user does not provide any ACT input, or the college has no ACT scores. keep -1
        }
    }

    if (userRank != null) {
        if (userRank == 10 || userRank == 25) rankResult = 4;
        else if (userRank == 50) rankResult = 2;
        else if (userRank == -50) rankResult = 1;
        else rankResult = 0;
    }

    // aggregate all evaluation here
    if (gpaResult > 0 && satResult > 0 && actResult > 0 && rankResult > 0) {
        let tempResult = gpaResult + satResult + actResult + rankResult;
        if (tempResult >= 14) finalResult = 4;
        else if (tempResult >= 8) finalResult = 2;
        else if (tempResult >= 6) finalResult = 1;
        else finalResult = 0;
    } else if (gpaResult > 0 && satResult > 0 && actResult > 0) {
        let tempResult = gpaResult + satResult + actResult;
        if (tempResult >= 10) finalResult = 4;
        else if (tempResult >= 6) finalResult = 2;
        else if (tempResult >= 4) finalResult = 1;
        else finalResult = 0;
    } else if (gpaResult > 0 && satResult > 0 && rankResult > 0) {
        let tempResult = gpaResult + satResult + rankResult;
        if (tempResult >= 10) finalResult = 4;
        else if (tempResult >= 6) finalResult = 2;
        else if (tempResult >= 4) finalResult = 1;
        else finalResult = 0;
    } else if (gpaResult > 0 && actResult > 0 && rankResult > 0) {
        let tempResult = gpaResult + actResult + rankResult;
        if (tempResult >= 10) finalResult = 4;
        else if (tempResult >= 6) finalResult = 2;
        else if (tempResult >= 4) finalResult = 1;
        else finalResult = 0;
    } else if (satResult > 0 && actResult > 0 && rankResult > 0) {
        let tempResult = satResult + actResult + rankResult;
        if (tempResult >= 10) finalResult = 4;
        else if (tempResult >= 6) finalResult = 2;
        else if (tempResult >= 4) finalResult = 1;
        else finalResult = 0;
    } else if (gpaResult > 0 && satResult > 0) {
        let tempResult = gpaResult + satResult;
        if (tempResult == 8) finalResult = 4;
        else if (tempResult >= 4) finalResult = 2;
        else if (tempResult == 3) finalResult = 1;
        else finalResult = 0;
    } else if (gpaResult > 0 && actResult > 0) {
        let tempResult = gpaResult + actResult;
        if (tempResult == 8) finalResult = 4;
        else if (tempResult >= 4) finalResult = 2;
        else if (tempResult == 3) finalResult = 1;
        else finalResult = 0;
    } else if (gpaResult > 0 && rankResult > 0) {
        let tempResult = gpaResult + rankResult;
        if (tempResult == 8) finalResult = 4;
        else if (tempResult >= 4) finalResult = 2;
        else if (tempResult == 3) finalResult = 1;
        else finalResult = 0;
    } else if (satResult > 0 && actResult > 0) {
        let tempResult = satResult + actResult;
        if (tempResult == 8) finalResult = 4;
        else if (tempResult >= 4) finalResult = 2;
        else if (tempResult == 3) finalResult = 1;
        else finalResult = 0;
    } else if (satResult > 0 && rankResult > 0) {
        let tempResult = satResult + rankResult;
        if (tempResult == 8) finalResult = 4;
        else if (tempResult >= 4) finalResult = 2;
        else if (tempResult == 3) finalResult = 1;
        else finalResult = 0;
    } else if (actResult > 0 && rankResult > 0) {
        let tempResult = actResult + rankResult;
        if (tempResult == 8) finalResult = 4;
        else if (tempResult >= 4) finalResult = 2;
        else if (tempResult == 3) finalResult = 1;
        else finalResult = 0;
    } else if (gpaResult > 0) {
        finalResult = gpaResult;
    } else if (satResult > 0) {
        finalResult = satResult;
    } else if (actResult > 0) {
        finalResult = actResult;
    } else if (rankResult > 0) {
        finalResult = rankResult;
    }
    return finalResult;
}
