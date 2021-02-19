/*
 * Abbreviation (9 of 19) and digital representation of 19 comprehensive factors:
 * 0 Rigor of secondary school record (Ri 3 levels);
 * 1 class rank (Cl 4 levels);
 * 2 GPA (Gp 4 levels);
 * 3 test scores (Te 3 levels);
 * 4 extracurricular activities (Ex 3 levels);
 * 5 talent (Ta 3 levels);
 * 6 first generation (Fi 2 levels);
 * 7 volunteer (Vo 3 levels);
 * 8 work experience (Wo 2 levels);
 * 9 essay; 10 recommendations; 11 interview; 12 character; 13 alumni; 14 geographical residence;
 * 15 state residency; 16 religion; 17 race; 18 interest.
 *
 * weight for each category:
 * 3 very important; 2 important; 1 considered
 *
 * for some top colleges use 80/60/40 as the comprehensive criteria; others 60/40/20
 * These top colleges have UNITID: 186131, 166027, 190150, 130794, 166683, 144050, 243744, 215062,
 * 110404, 147767, 162928, 198419, 182670, 217156, 221999, 227757, 190415
 *
 * entry: MatchData row; input: user input with a structure like below
 * {
 *  Ri, Cl, Gp, Te, Ex, Ta, Fi, Vo, Wo
 * }
 * output: 4 highly recommend; 2 recommend; 1 neutral; 0 not recommend; -1 no result
 */
function comprehensiveMatch(entry, userInput) {
    const collegeComFactors = entry.Factors;
    if (collegeComFactors == null) return -1;
    // levels of each comprehensive factor
    const coef_Ri = 3,
        coef_Cl = 4,
        coef_Gp = 4,
        coef_Te = 3,
        coef_Ex = 3,
        coef_Ta = 3,
        coef_Fi = 2,
        coef_Vo = 3,
        coef_Wo = 2;
    const vCoef = 3,
        iCoef = 2,
        cCoef = 1; // weight of very important/important/considered
    const vFactors = collegeComFactors.v; //very important factor array
    const iFactors = collegeComFactors.i; //important factor array
    const cFactors = collegeComFactors.c; //considered factor array
    let fullScore = 0;
    let score = 0;
    const top_colleges = [
        "186131",
        "166027",
        "190150",
        "130794",
        "166683",
        "144050",
        "243744",
        "215062",
        "110404",
        "147767",
        "162928",
        "198419",
        "182670",
        "217156",
        "221999",
        "227757",
        "190415",
    ];

    // calculate full score firstly
    if (vFactors != null)
        for (let i = 0; i < vFactors.length; i++) {
            switch (vFactors[i]) {
                case 0:
                    fullScore += coef_Ri * vCoef;
                    score += userInput.Ri == null ? 0 : userInput.Ri * vCoef;
                    break;
                case 1:
                    fullScore += coef_Cl * vCoef;
                    score += userInput.Cl == null ? 0 : userInput.Cl * vCoef;
                    break;
                case 2:
                    fullScore += coef_Gp * vCoef;
                    score += userInput.Gp == null ? 0 : userInput.Gp * vCoef;
                    break;
                case 3:
                    fullScore += coef_Te * vCoef;
                    score += userInput.Te == null ? 0 : userInput.Te * vCoef;
                    break;
                case 4:
                    fullScore += coef_Ex * vCoef;
                    score += userInput.Ex == null ? 0 : userInput.Ex * vCoef;
                    break;
                case 5:
                    fullScore += coef_Ta * vCoef;
                    score += userInput.Ta == null ? 0 : userInput.Ta * vCoef;
                    break;
                case 6:
                    fullScore += coef_Fi * vCoef;
                    score += userInput.Fi == null ? 0 : userInput.Fi * vCoef;
                    break;
                case 7:
                    fullScore += coef_Vo * vCoef;
                    score += userInput.Vo == null ? 0 : userInput.Vo * vCoef;
                    break;
                case 8:
                    fullScore += coef_Wo * vCoef;
                    score += userInput.Wo == null ? 0 : userInput.Wo * vCoef;
                    break;
                default:
                    break;
            }
        }
    if (iFactors != null)
        for (let i = 0; i < iFactors.length; i++) {
            switch (iFactors[i]) {
                case 0:
                    fullScore += coef_Ri * iCoef;
                    score += userInput.Ri == null ? 0 : userInput.Ri * iCoef;
                    break;
                case 1:
                    fullScore += coef_Cl * iCoef;
                    score += userInput.Cl == null ? 0 : userInput.Cl * iCoef;
                    break;
                case 2:
                    fullScore += coef_Gp * iCoef;
                    score += userInput.Gp == null ? 0 : userInput.Gp * iCoef;
                    break;
                case 3:
                    fullScore += coef_Te * iCoef;
                    score += userInput.Te == null ? 0 : userInput.Te * iCoef;
                    break;
                case 4:
                    fullScore += coef_Ex * iCoef;
                    score += userInput.Ex == null ? 0 : userInput.Ex * iCoef;
                    break;
                case 5:
                    fullScore += coef_Ta * iCoef;
                    score += userInput.Ta == null ? 0 : userInput.Ta * iCoef;
                    break;
                case 6:
                    fullScore += coef_Fi * iCoef;
                    score += userInput.Fi == null ? 0 : userInput.Fi * iCoef;
                    break;
                case 7:
                    fullScore += coef_Vo * iCoef;
                    score += userInput.Vo == null ? 0 : userInput.Vo * iCoef;
                    break;
                case 8:
                    fullScore += coef_Wo * iCoef;
                    score += userInput.Wo == null ? 0 : userInput.Wo * iCoef;
                    break;
                default:
                    break;
            }
        }
    if (cFactors != null)
        for (let i = 0; i < cFactors.length; i++) {
            switch (cFactors[i]) {
                case 0:
                    fullScore += coef_Ri * cCoef;
                    score += userInput.Ri == null ? 0 : userInput.Ri * cCoef;
                    break;
                case 1:
                    fullScore += coef_Cl * cCoef;
                    score += userInput.Cl == null ? 0 : userInput.Cl * cCoef;
                    break;
                case 2:
                    fullScore += coef_Gp * cCoef;
                    score += userInput.Gp == null ? 0 : userInput.Gp * cCoef;
                    break;
                case 3:
                    fullScore += coef_Te * cCoef;
                    score += userInput.Te == null ? 0 : userInput.Te * cCoef;
                    break;
                case 4:
                    fullScore += coef_Ex * cCoef;
                    score += userInput.Ex == null ? 0 : userInput.Ex * cCoef;
                    break;
                case 5:
                    fullScore += coef_Ta * cCoef;
                    score += userInput.Ta == null ? 0 : userInput.Ta * cCoef;
                    break;
                case 6:
                    fullScore += coef_Fi * cCoef;
                    score += userInput.Fi == null ? 0 : userInput.Fi * cCoef;
                    break;
                case 7:
                    fullScore += coef_Vo * cCoef;
                    score += userInput.Vo == null ? 0 : userInput.Vo * cCoef;
                    break;
                case 8:
                    fullScore += coef_Wo * cCoef;
                    score += userInput.Wo == null ? 0 : userInput.Wo * cCoef;
                    break;
                default:
                    break;
            }
        }

    if (fullScore == 0) return -1;
    const result = score / fullScore;
    if (top_colleges.includes(entry.UNITID)) {
        // top college use 80/60/40
        if (result >= 0.8) return 4;
        else if (result >= 0.6) return 2;
        else if (result >= 0.4) return 1;
        else return 0;
    } else {
        // use 60/40/20
        if (result >= 0.6) return 4;
        else if (result >= 0.4) return 2;
        else if (result >= 0.2) return 1;
        else return 0;
    }
}
