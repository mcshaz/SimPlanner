"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function commonMissingExpandRoot(missingExpands) {
    var i;
    var minNotMissJ = Math.min.apply(null, missingExpands.map(function (el) { return el.missing; })) - 1;
    var lastCommonRootIndex;
    var foundDif = false;
    var currentVar;
    for (lastCommonRootIndex = 0; lastCommonRootIndex <= minNotMissJ; lastCommonRootIndex++) {
        currentVar = null;
        for (i = 0; i < missingExpands.length; i++) {
            if (missingExpands[i].props.length > lastCommonRootIndex) {
                if (!currentVar) {
                    currentVar = missingExpands[i].prop[j];
                }
                else if (currentVar != missingExpands[i].prop[j]) {
                    lastCommonRootIndex--;
                    foundDif = true;
                    break;
                }
            }
        }
        if (foundDif) {
            break;
        }
    }
    return missingExpands.map(function (el) { return el.props.slice(lastCommonRootIndex).join('.'); });
}
self.fetchByKey = function (key, argObj) {
    var ent = executeKeyQueryLocally(key, argObj);
    if (ent) {
        var missing = missingExpands(ent, argObj.expand);
        if (missing.length) {
            log.debug({ msg: 'missing expand properties not found: querying server', data: missing });
            return executeKeyQuery(key, {
                select: missing.map(function (el) {
                    return el.props.join('.');
                }).join(',')
            });
        }
        else {
            return $q.when(ent);
        }
    }
    else {
        return executeKeyQuery(key, argObj);
    }
};
function missingExpands(entity, expand) {
    var returnVar = [];
    if (!expand) {
        return returnVar;
    }
    if (!Array.isArray(expand)) {
        expand = [expand];
    }
    expand.forEach(function (el) {
        var props = el.split('.');
        var currentProp = entity;
        var i = 0;
        for (; i < props.length; i++) {
            if (Array.isArray(currentProp)) {
                if (!currentProp.length) {
                    break;
                }
                currentProp = currentProp[0][props[i]];
            }
            else {
                currentProp = currentProp[props[i]];
            }
            if (!currentProp) {
                returnVar.push({ props: props, missing: i });
                break;
            }
        }
    });
    return returnVar;
}
;
//# sourceMappingURL=ToDoUtilities.js.map