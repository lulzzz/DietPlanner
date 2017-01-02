library(topsis)
library(mco)

d <- matrix(data, nrow = individualsCount[1])

dn <- normalizeFront(d)

topsisResult = topsis(dn, weights, impacts)

scores = topsisResult$score
ranks = topsisResult$rank
rows = topsisResult$alt.row