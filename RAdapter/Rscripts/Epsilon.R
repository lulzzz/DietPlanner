library(mco)

m = matrix(data, individuals[1], criterions[1], TRUE)
tf = matrix(data_tf, individuals_tf[1], criterions_tf[1], TRUE)

eps = epsilonIndicator(m, tf)
