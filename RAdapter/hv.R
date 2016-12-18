library(mco)

m = matrix(data, individuals[1], criterions[1], TRUE)

dhv = dominatedHypervolume(normalizeFront(m))
