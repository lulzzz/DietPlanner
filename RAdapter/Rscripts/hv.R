library(mco)

m = matrix(data, individuals[1], criterions[1], TRUE)

if(useReferecnePoint) {
  dhv = dominatedHypervolume(m, referencePoint)
} else {
  dhv = dominatedHypervolume(m)
}