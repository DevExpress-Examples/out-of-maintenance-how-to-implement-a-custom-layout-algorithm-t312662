#Region "#CustomLayoutAlgorithmImpl"
Imports DevExpress.Xpf.TreeMap
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports System

Namespace CustomLayoutAlgorithmSample
    Friend Class CustomSliceAndDiceLayoutAlgorithm
        Inherits TreeMapLayoutAlgorithmBase
        Implements IComparer(Of ITreeMapLayoutItem)

        ' Cut the slice depending on the non-filled space width/height ratio.
        Public Overrides Sub Calculate(ByVal items As IList(Of ITreeMapLayoutItem), ByVal size As Size, ByVal groupLevel As Integer)
            Dim unlayoutedItemsWeight As Double = 0
            For Each item In items
                unlayoutedItemsWeight += item.Weight
            Next item

            Dim sortedItems = items.ToList()
            sortedItems.Sort(Me)

            Dim emptySpace As New Rect(0, 0, size.Width, size.Height)
            For Each item In sortedItems
                Dim itemWidth As Double
                Dim itemHeight As Double

                Dim newEmptySpaceX As Double
                Dim newEmptySpaceY As Double
                Dim newEmptySpaceWidth As Double
                Dim newEmptySpaceHeight As Double

                If emptySpace.Width / emptySpace.Height > 1.0 Then
                    itemWidth = emptySpace.Width * item.Weight / unlayoutedItemsWeight
                    itemHeight = emptySpace.Height

                    newEmptySpaceX = emptySpace.X + itemWidth
                    newEmptySpaceY = emptySpace.Y
                    newEmptySpaceHeight = emptySpace.Height

                    newEmptySpaceWidth = emptySpace.Width - itemWidth
                    newEmptySpaceWidth = If(newEmptySpaceWidth < 0, 0, newEmptySpaceWidth)
                Else
                    itemWidth = emptySpace.Width
                    itemHeight = emptySpace.Height * item.Weight / unlayoutedItemsWeight

                    newEmptySpaceX = emptySpace.X
                    newEmptySpaceY = emptySpace.Y + itemHeight
                    newEmptySpaceWidth = emptySpace.Width

                    newEmptySpaceHeight = emptySpace.Height - itemHeight
                    newEmptySpaceHeight = If(newEmptySpaceHeight < 0, 0, newEmptySpaceHeight)
                End If
                item.Layout = New Rect(emptySpace.X, emptySpace.Y, itemWidth, itemHeight)
                emptySpace = New Rect(newEmptySpaceX, newEmptySpaceY, newEmptySpaceWidth, newEmptySpaceHeight)
                unlayoutedItemsWeight -= item.Weight
            Next item
        End Sub

        Public Function Compare(ByVal x As ITreeMapLayoutItem, ByVal y As ITreeMapLayoutItem) As Integer Implements IComparer(Of ITreeMapLayoutItem).Compare
            If x.Weight > y.Weight Then
                Return -1
            ElseIf x.Weight < y.Weight Then
                Return 1
            Else
                Return 0
            End If
        End Function

        Protected Overrides Function CreateObject() As TreeMapDependencyObject
            Return New CustomSliceAndDiceLayoutAlgorithm()
        End Function
    End Class
End Namespace
#End Region ' #CustomLayoutAlgorithmImpl